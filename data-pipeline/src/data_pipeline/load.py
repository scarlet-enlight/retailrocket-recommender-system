import io

import pandas as pd
import psycopg2
from psycopg2.extensions import connection, cursor

from data_pipeline.config import db_config
from data_pipeline.extract import extract_all
from data_pipeline.utils.logger import setup_logger

load_logger = setup_logger(name="load_logger")


def load_historical(conn: connection, data: dict[str, pd.DataFrame]) -> None:
    """One of the main components of the ELT pipeline.

    Loads historical data into database
    """

    with conn, conn.cursor() as cur:
        # Truncate historical schema before loading
        assert_dependents_empty(cur)
        truncate_historical(cur)

        # Load categories
        categories = data["categories"]
        rows = load_categories(cur, categories)
        check_count(cur, "categories", len(categories), rows)

        # Load visitors
        events = data["events"]
        rows = load_visitors(cur, events)

        expected = events["visitorid"].nunique()
        check_count(cur, "visitors", expected, rows)

        # Load items
        props = data["items"]
        rows = load_items(cur, events, props)

        expected = events["itemid"].nunique()
        check_count(cur, "items", expected, rows)


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


def load_visitors(cur: cursor, events_df: pd.DataFrame) -> int:
    """Bulk-load visitors via COPY and return the number of rows loaded."""

    visitors = (
        events_df[["visitorid"]]
        .drop_duplicates()
        .dropna()
        .reset_index(drop=True)
        .rename(columns={"visitorid": "visitor_id"})
    )

    load_logger.info("Started loading visitors...")
    sql = """
        COPY historical.visitors (visitor_id)
        FROM STDIN WITH (FORMAT CSV, HEADER FALSE)
    """
    buffer = io.StringIO()
    visitors.to_csv(buffer, index=False, header=False)
    buffer.seek(0)
    cur.copy_expert(sql, buffer)

    return cur.rowcount


def load_items(cur: cursor, events_df: pd.DataFrame, props_df: pd.DataFrame) -> int:
    """Bulk-load items via COPY and return the number of rows loaded."""

    item_ids = events_df[["itemid"]].drop_duplicates().dropna().reset_index(drop=True)

    latest_props = (
        props_df[props_df["property"].isin(["categoryid", "available"])]
        .sort_values("timestamp")
        .drop_duplicates(["itemid", "property"], keep="last")
    )
    props_wide = latest_props.pivot(index="itemid", columns="property", values="value")

    # Type casting
    props_wide["categoryid"] = pd.to_numeric(props_wide["categoryid"]).astype("Int32")

    props_wide["available"] = (
        props_wide["available"].map({"1": True, "0": False}).astype("boolean")
    )

    items = item_ids.merge(
        props_wide.reset_index(), on="itemid", how="left", validate="one_to_one"
    )

    # Check for categories orphans
    cur.execute("SELECT category_id FROM historical.categories")
    known_categories = {row[0] for row in cur.fetchall()}

    missing_categories = set(items["categoryid"].dropna()) - known_categories
    if missing_categories:
        raise ValueError(
            f"items: unknown category_id values: {sorted(missing_categories)[:10]}"
        )

    # Load items to database
    load_logger.info("Started loading items...")

    items = items.rename(columns={"itemid": "item_id", "categoryid": "category_id"})[
        ["item_id", "category_id", "available"]
    ]
    sql = """
        COPY historical.items (item_id, category_id, available)
        FROM STDIN WITH (FORMAT csv, HEADER FALSE, NULL '')
    """

    buffer = io.StringIO()
    items.to_csv(buffer, index=False, header=False, na_rep="")
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
    """Raise RuntimeError if found dependencies."""
    for table in ("shop.products", "ml.recommendation_rules"):
        cur.execute(f"SELECT COUNT(*) FROM {table}")

        row = cur.fetchone()
        if row and row[0] > 0:
            raise RuntimeError(f"{table} is not empty, refusing to TRUNCATE CASCADE")


def check_count(cur: cursor, table: str, expected: int, loaded: int) -> None:
    """Raise if COPY rowcount or the table size differs from the expected count."""

    cur.execute(f"SELECT COUNT(*) FROM historical.{table}")
    row = cur.fetchone()
    in_db = row[0] if row else None

    if loaded != expected or in_db != expected:
        raise ValueError(
            f"{table}: expected {expected}, loaded {loaded}, in db {in_db}"
        )

    load_logger.info("%s: loaded %d rows", table, loaded)


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
