import argparse
import sys

import psycopg2
from psycopg2.extensions import connection, cursor

from data_pipeline.config import db_config
from data_pipeline.utils.logger import setup_logger
from data_pipeline.utils.validators import DataQualityError

logger = setup_logger(name="seed_shop_logger")

SEED_SQL = """
    WITH recommendation_items AS (
        SELECT if_item_id AS item_id FROM ml.recommendation_rules
        UNION
        SELECT then_item_id FROM ml.recommendation_rules
    ),
    recommendation_items_with_category AS (
        SELECT item_id
        FROM historical.items i
        WHERE i.category_id IS NOT NULL
            AND i.item_id IN (SELECT ri.item_id FROM recommendation_items ri)
    ),
    popular_available_items AS (
        SELECT
            i.item_id,
            COUNT(e.event_type) AS event_count
        FROM historical.items i
        JOIN historical.events e
            USING(item_id)
        WHERE i.available = TRUE
            AND i.category_id IS NOT NULL
            AND e.event_type IN ('addtocart', 'transaction')
            AND NOT EXISTS (
                SELECT 1 FROM recommendation_items rr WHERE rr.item_id = i.item_id
            )
        GROUP BY i.item_id
        ORDER BY event_count DESC, i.item_id
        LIMIT %s
    ),
    selected_items AS (
        SELECT item_id FROM recommendation_items_with_category 
        UNION
        SELECT item_id FROM popular_available_items
    )
    INSERT INTO shop.products (item_id, name, price, category_id)
    SELECT
        s.item_id,
        'Item ' || s.item_id,
        ROUND(s.item_id %% 494.99 + 5, 2),
        i.category_id
    FROM selected_items s
    JOIN historical.items i
        USING(item_id)
    WHERE NOT EXISTS (SELECT 1 FROM shop.products p WHERE p.item_id = i.item_id);
"""

CHECK_SQL = """
    SELECT COUNT(*)
    FROM ml.recommendation_rules r
    WHERE
        NOT EXISTS (SELECT 1 FROM shop.products p WHERE p.item_id = r.then_item_id)
        OR NOT EXISTS (SELECT 1 FROM shop.products p WHERE p.item_id = r.if_item_id);
"""


def run_seed(limit: int) -> None:
    conn = psycopg2.connect(db_config.connection_string)
    try:
        seed_products(conn, limit)
    finally:
        conn.close()


def seed_products(conn: connection, limit: int) -> int:
    """Fill shop.products with rule items and the most popular historical items."""

    with conn, conn.cursor() as cur:
        _warn_if_no_rules(cur)
        inserted = _insert_products(cur, limit)
        _check_chain(cur)
    logger.info("Inserted %d products", inserted)
    return inserted


def _check_chain(cur: cursor) -> None:
    cur.execute(CHECK_SQL)
    row = cur.fetchone()
    if row and row[0] > 0:
        raise DataQualityError(
            f"{row[0]} recommendation rules reference items missing from shop.products"
        )


def _insert_products(cur: cursor, limit: int) -> int:
    cur.execute(SEED_SQL, (limit,))
    return cur.rowcount


def _warn_if_no_rules(cur: cursor) -> None:
    cur.execute("SELECT COUNT(*) FROM ml.recommendation_rules;")
    row = cur.fetchone()
    if row and row[0] == 0:
        logger.warning("recommendation_rules table is empty")


def _parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()

    parser.add_argument(
        "--limit",
        type=int,
        default=200,
        help="Number of popular available items to add (in addition to all rule items)",
    )

    args = parser.parse_args()
    if args.limit < 0:
        parser.error("--limit must be >= 0")
    return args


def main() -> int:
    limit = _parse_args().limit

    try:
        logger.info("Seed shop script started with limit %d for popular items", limit)
        run_seed(limit)
        logger.info("Seed shop script finished")
    except Exception:
        logger.exception("Seed shop script failed")
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
