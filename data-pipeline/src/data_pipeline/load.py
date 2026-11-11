import io

import pandas as pd
import psycopg2
from psycopg2.extensions import connection, cursor

from data_pipeline.config import db_config
from data_pipeline.extract import extract_all
from data_pipeline.utils.logger import setup_logger

load_logger = setup_logger(name="load_logger")


def load_historical(conn: connection, data: dict[str, pd.DataFrame]):
    """One of main component of ELT pipeline.

    Loads historical data into database
    """

    with conn, conn.cursor() as cur:
        # Truncate historical schema before loading
        assert_dependents_empty(cur)
        truncate_historical(cur)

        # Load categories
        categories = data["categories"]
        rows = load_categories(cur, categories)

        cur.execute("SELECT COUNT(*) FROM historical.categories")
        count_tuple = cur.fetchone()
        if not count_tuple:
            raise ValueError("No categories data was loaded")
        in_db = count_tuple[0]

        if rows != len(categories) or in_db != len(categories):
            raise ValueError(
                f"categories: expected {len(categories)}, loaded {rows}, in db {in_db}"
            )

        load_logger.info(f"Categories have been loaded, rows: {rows}")


def load_categories(cur: cursor, df: pd.DataFrame) -> int:
    """Bulk-load the category tree via COPY and return the number of rows loaded."""

    # Rename df columns to match db schema
    df = df.rename(columns={"categoryid": "category_id", "parentid": "parent_id"})

    df = df[["category_id", "parent_id"]]

    # Check if in categories any orphans
    orphans = df[df["parent_id"].notna() & ~df["parent_id"].isin(df["category_id"])]
    if len(orphans) > 0:
        raise ValueError(f"Orphans in category: {len(orphans)}")

    # Load data into database
    load_logger.info("Started loading categories...")
    sql = """
            COPY historical.categories (category_id, parent_id)
            FROM STDIN WITH (FORMAT CSV, HEADER FALSE, NULL '')
    """
    buffer = io.StringIO()
    df.to_csv(buffer, index=False, header=False, na_rep="")
    buffer.seek(0)
    cur.copy_expert(sql, buffer)

    return cur.rowcount


def truncate_historical(cur: cursor) -> None:
    """Empty all historical tables.

    Uses CASCADE, so it also empties shop.products and ml.recommendation_rules
    (guarded by assert_dependents_empty).
    """

    cur.execute("""
    TRUNCATE 
        historical.categories,
        historical.events,
        historical.items,
        historical.transaction_items,
        historical.transactions,
        historical.visitors
    RESTART IDENTITY CASCADE;
    """)


def assert_dependents_empty(cur: cursor) -> None:
    for table in ("shop.products", "ml.recommendation_rules"):
        cur.execute(f"SELECT COUNT(*) FROM {table}")

        row = cur.fetchone()
        if row and row[0] > 0:
            raise RuntimeError(f"{table} is not empty, refusing to TRUNCATE CASCADE")


if __name__ == "__main__":
    extracted_data = extract_all()

    conn = psycopg2.connect(
        host=db_config.host,
        port=db_config.port,
        dbname=db_config.name,
        user=db_config.user,
        password=db_config.password,
    )

    try:
        load_historical(conn, extracted_data)
    finally:
        conn.close()
