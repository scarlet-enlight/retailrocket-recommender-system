import duckdb
import pytest

from data_pipeline.transform import BASKETS_SQL

JAN_1_2020_MS = 1_577_836_800_000


@pytest.fixture()
def db():
    with duckdb.connect() as con:
        con.execute("ATTACH ':memory:' AS pg")
        con.execute("CREATE SCHEMA pg.historical")
        con.execute("""
            CREATE TABLE pg.historical.events (
                visitor_id INT NOT NULL,
                item_id INT NOT NULL,
                event_type VARCHAR(20) NOT NULL
                    CHECK (event_type IN ('view', 'addtocart', 'transaction')),
                ts BIGINT NOT NULL
            )
        """)
        con.execute("""
            CREATE TABLE pg.historical.transaction_items (
                transaction_id INT,
                item_id INT,
                PRIMARY KEY (transaction_id, item_id)
            )
        """)
        yield con


def insert_events(con, rows):
    con.executemany("INSERT INTO pg.historical.events VALUES (?, ?, ?, ?)", rows)


def insert_transaction_items(con, rows):
    con.executemany("INSERT INTO pg.historical.transaction_items VALUES (?, ?)", rows)


def build_baskets(con):
    return sorted(con.execute(BASKETS_SQL).fetchall())


def test_client_add_item_twice(db):
    insert_events(
        db,
        [(1, 1, "addtocart", JAN_1_2020_MS), (1, 1, "addtocart", JAN_1_2020_MS + 1000)],
    )

    baskets = build_baskets(db)
    expected = [("session_cart", "1_2020-01-01", 1)]

    assert baskets == expected


def test_view_not_in_baskets(db):
    insert_events(
        db, [(1, 1, "addtocart", JAN_1_2020_MS), (2, 2, "view", JAN_1_2020_MS + 10000)]
    )

    baskets = build_baskets(db)
    expected = [("session_cart", "1_2020-01-01", 1)]

    assert baskets == expected


def test_utс_day_edge(db):
    insert_events(
        db,
        [
            (1, 1, "addtocart", JAN_1_2020_MS + 86_399_000),
            (1, 1, "addtocart", JAN_1_2020_MS + 86_401_000),
        ],
    )

    baskets = build_baskets(db)
    expected = [
        ("session_cart", "1_2020-01-01", 1),
        ("session_cart", "1_2020-01-02", 1),
    ]

    assert baskets == expected


def test_different_customers_same_item(db):
    insert_events(
        db,
        [
            (1, 1, "addtocart", JAN_1_2020_MS),
            (2, 1, "addtocart", JAN_1_2020_MS + 1000),
        ],
    )

    baskets = build_baskets(db)
    expected = [
        ("session_cart", "1_2020-01-01", 1),
        ("session_cart", "2_2020-01-01", 1),
    ]

    assert baskets == expected


def test_transaction_basket(db):
    insert_transaction_items(db, [(7, 3)])

    baskets = build_baskets(db)
    expected = [("transaction", "7", 3)]

    assert baskets == expected


def test_addtocart_and_transaction_events(db):
    insert_events(
        db,
        [
            (1, 1, "addtocart", JAN_1_2020_MS),
            (1, 2, "transaction", JAN_1_2020_MS + 1000),
        ],
    )

    baskets = build_baskets(db)
    expected = [
        ("session_cart", "1_2020-01-01", 1),
        ("session_cart", "1_2020-01-01", 2),
    ]

    assert baskets == expected
