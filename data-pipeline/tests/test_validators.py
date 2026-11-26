import duckdb
import pytest

from data_pipeline.utils import validators
from data_pipeline.utils.validators import DataQualityError, validate_baskets


def sql_literal(value):
    if value is None:
        return "NULL"
    return repr(value)


def write_baskets(path, rows):
    """Write rows [(basket_type, basket_id, item_id), ...] to a parquet file."""

    values = ", ".join(
        f"({sql_literal(basket_type)}, {sql_literal(basket_id)}, "
        f"{sql_literal(item_id)})"
        for basket_type, basket_id, item_id in rows
    )
    duckdb.sql(f"""
        COPY (
            SELECT
                basket_type::VARCHAR AS basket_type,
                basket_id::VARCHAR AS basket_id,
                item_id::INT AS item_id
            FROM (VALUES {values}) AS t(basket_type, basket_id, item_id)
        )
        TO '{path}' (FORMAT parquet)
    """)


@pytest.fixture(autouse=True)
def small_limits(monkeypatch):
    monkeypatch.setattr(
        validators, "MIN_BASKETS", {"session_cart": 1, "transaction": 1}
    )


def test_valid_file_passes(tmp_path):
    path = tmp_path / "baskets.parquet"

    write_baskets(path, [("session_cart", "1_2020-01-01", 1), ("transaction", "2", 2)])

    validate_baskets(path)


@pytest.mark.parametrize(
    "rows, column",
    [
        ([(None, "1_2020-01-01", 1), ("transaction", "2", 2)], "basket_type"),
        ([("session_cart", None, 1), ("transaction", "2", 2)], "basket_id"),
        ([("session_cart", "1_2020-01-01", None), ("transaction", "2", 2)], "item_id"),
    ],
)
def test_null_raises(tmp_path, rows, column):
    path = tmp_path / "baskets.parquet"
    write_baskets(path, rows)

    with pytest.raises(DataQualityError, match=column):
        validate_baskets(path)


def test_duplicates_raises(tmp_path):
    path = tmp_path / "baskets.parquet"
    write_baskets(
        path,
        [
            ("session_cart", "1_2020-01-01", 1),
            ("session_cart", "1_2020-01-01", 1),
            ("transaction", "2", 2),
        ],
    )

    with pytest.raises(DataQualityError, match="duplicate"):
        validate_baskets(path)


def test_extra_type_raises(tmp_path):
    path = tmp_path / "baskets.parquet"
    write_baskets(
        path,
        [
            ("session_cart", "1_2020-01-01", 1),
            ("transaction", "2", 2),
            ("test_type", "3", 3),
        ],
    )

    with pytest.raises(DataQualityError, match="mismatch"):
        validate_baskets(path)


@pytest.mark.parametrize(
    "rows, missing_type",
    [
        (
            [("session_cart", "1_2020-01-01", 1), ("session_cart", "2_2020-01-01", 2)],
            "transaction",
        ),
        ([("transaction", "1", 1), ("transaction", "2", 2)], "session_cart"),
    ],
)
def test_missing_type_raises(tmp_path, rows, missing_type):
    path = tmp_path / "baskets.parquet"
    write_baskets(path, rows)

    with pytest.raises(DataQualityError, match=missing_type):
        validate_baskets(path)


def test_too_few_baskets_raises(tmp_path, monkeypatch):
    path = tmp_path / "baskets.parquet"
    write_baskets(path, [("session_cart", "1_2020-01-01", 1), ("transaction", "2", 2)])

    monkeypatch.setattr(
        validators, "MIN_BASKETS", {"session_cart": 5, "transaction": 5}
    )

    with pytest.raises(DataQualityError, match="too few"):
        validate_baskets(path)


@pytest.mark.parametrize(
    "query, message",
    [
        (
            "SELECT 'session_cart'::VARCHAR AS basket_type, '1'::VARCHAR AS basket_id",
            "columns mismatch",
        ),
        (
            """SELECT
                'session_cart'::VARCHAR AS basket_type,
                '1'::VARCHAR AS basket_id,
                1::INT AS item_id,
                0::INT AS test_column
            """,
            "columns mismatch",
        ),
        (
            """SELECT
                'session_cart'::VARCHAR AS basket_type,
                '1'::VARCHAR AS basket_id,
                1::BIGINT AS item_id
            """,
            "types mismatched",
        ),
    ],
)
def test_bad_structure_raises(tmp_path, query, message):
    path = tmp_path / "baskets.parquet"

    duckdb.sql(f"COPY ({query}) TO '{path}' (FORMAT parquet)")

    with pytest.raises(DataQualityError, match=message):
        validate_baskets(path)
