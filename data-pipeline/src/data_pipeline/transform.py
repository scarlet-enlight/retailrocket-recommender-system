from pathlib import Path

import duckdb

from data_pipeline.config import BASKETS_PARQUET_PATH, db_config
from data_pipeline.utils.logger import setup_logger
from data_pipeline.utils.validators import validate_baskets

transform_logger = setup_logger(name="transform_logger")

SESSION_CART_SQL = """
    SELECT
        'session_cart' AS basket_type,
        visitor_id || '_' || epoch_ms(ts)::DATE AS basket_id,
        item_id
    FROM pg.historical.events
    WHERE event_type IN ('addtocart', 'transaction')
    GROUP BY ALL
"""

TRANSACTION_SQL = """
    SELECT
        'transaction' AS basket_type,
        transaction_id::VARCHAR AS basket_id,
        item_id
    FROM pg.historical.transaction_items
"""

BASKETS_SQL = f"""
    WITH baskets AS (
        {SESSION_CART_SQL}
        UNION ALL
        {TRANSACTION_SQL}
    )
    SELECT * FROM baskets
    ORDER BY basket_type, basket_id, item_id
"""


def run_transform(pg_dsn: str, out_path: Path) -> None:
    con = connect_to_postgres(pg_dsn)

    with con:
        out_path.parent.mkdir(parents=True, exist_ok=True)
        tmp = out_path.with_suffix(".parquet.tmp")

        try:
            rows = export_baskets_to_parquet(con, tmp)
            transform_logger.info("Baskets written: %d rows", rows)
            validate_baskets(tmp)
            transform_logger.info("Baskets validated")
            tmp.replace(out_path)
        finally:
            tmp.unlink(missing_ok=True)


def connect_to_postgres(pg_dsn: str) -> duckdb.DuckDBPyConnection:
    con = duckdb.connect()

    con.execute("SET memory_limit='4GB'; SET threads=4")
    con.execute("INSTALL postgres; LOAD postgres")
    con.execute(f"ATTACH '{pg_dsn}' AS pg (TYPE postgres, READ_ONLY)")

    return con


def export_baskets_to_parquet(con: duckdb.DuckDBPyConnection, path: Path) -> int:
    con.execute(f"COPY ({BASKETS_SQL}) TO '{path}' (FORMAT parquet)")

    return con.rowcount


if __name__ == "__main__":
    pg_dsn = f"host={db_config.host} port={db_config.port} dbname={db_config.name} user={db_config.user} password={db_config.password}"
    out_path = BASKETS_PARQUET_PATH

    run_transform(pg_dsn, out_path)
