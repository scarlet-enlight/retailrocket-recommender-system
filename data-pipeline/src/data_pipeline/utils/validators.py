from pathlib import Path

import duckdb

from data_pipeline.config import BASKETS_PARQUET_PATH

EXPECTED_TYPES: dict[str, str] = {
    "basket_type": "VARCHAR",
    "basket_id": "VARCHAR",
    "item_id": "INTEGER",
}
EXPECTED_COLUMNS = set(EXPECTED_TYPES)
EXPECTED_BASKET_TYPES = {"transaction", "session_cart"}
# Nearly 60% of expected values (43 265 session_carts and 17 672 transactions)
MIN_BASKETS = {"session_cart": 25_000, "transaction": 10_000}


class DataQualityError(Exception):
    """Raised when the file breaks the expected contract."""


def validate_baskets(path: Path) -> None:
    """Raise if the baskets file breaks the contract"""

    with duckdb.connect() as con:
        schema = _read_schema(con, path)
        _check_columns(list(schema))
        _check_types(schema)

        _check_no_null(_count_nulls(con, path))
        _check_no_dups(_count_dups(con, path))
        _check_baskets_types(_get_basket_types(con, path))
        _check_min_baskets(_count_baskets(con, path))


def _check_columns(columns: list[str]) -> None:
    """Check if baskets file has correct columns"""
    missing = EXPECTED_COLUMNS - set(columns)
    extra = set(columns) - EXPECTED_COLUMNS

    if missing or extra:
        raise DataQualityError(
            f"columns mismatch: missing={sorted(missing)}, extra={sorted(extra)}"
        )


def _check_types(types: dict[str, str]) -> None:
    """Check if baskets file has correct data types"""

    diff = {
        col: (EXPECTED_TYPES[col], types.get(col))
        for col in EXPECTED_TYPES
        if EXPECTED_TYPES[col] != types.get(col)
    }

    if diff:
        raise DataQualityError(f"data types mismatched (expected, received): {diff}")


def _check_baskets_types(basket_types: list[str]) -> None:
    """Check if baskets types have only available values and both exist"""

    missing = EXPECTED_BASKET_TYPES - set(basket_types)
    extra = set(basket_types) - EXPECTED_BASKET_TYPES

    if missing or extra:
        raise DataQualityError(
            f"basket types values mismatch: missing={sorted(missing)}, extra={sorted(extra)}"
        )


def _check_no_null(null_count: dict[str, int]) -> None:
    """Check if there is no NULL values in all columns"""

    null_cols = {col: nulls for col, nulls in null_count.items() if nulls > 0}

    if null_cols:
        raise DataQualityError(f"there are NULL values in columns: {null_cols}")


def _check_no_dups(dups_count: int) -> None:
    """Check if baskets has no duplicate rows"""

    if dups_count > 0:
        raise DataQualityError(f"baskets has {dups_count} duplicate rows")


def _check_min_baskets(baskets_count: dict[str, int]) -> None:
    """Raise if a basket type has too few baskets"""

    too_small_baskets = {
        basket_type: (baskets_count.get(basket_type, 0), minimum)
        for basket_type, minimum in MIN_BASKETS.items()
        if baskets_count.get(basket_type, 0) < minimum
    }

    if too_small_baskets:
        raise DataQualityError(
            f"too few baskets (received, expected): {too_small_baskets}"
        )


def _read_schema(con: duckdb.DuckDBPyConnection, path: Path) -> dict[str, str]:
    """Return {column_name: column_type} for the file."""

    rows = con.execute(f"DESCRIBE SELECT * FROM read_parquet('{path}')").fetchall()
    return {name: col_type for name, col_type, *_ in rows}


def _count_dups(con: duckdb.DuckDBPyConnection, path: Path) -> int:
    """Return the number of duplicate rows."""

    row = con.execute(f"""
        SELECT
            COUNT(*),
            COUNT(DISTINCT ROW(basket_type, basket_id, item_id))
        FROM read_parquet('{path}')
    """).fetchone()

    if not row:
        raise ValueError("_count_dups function extracted no data")

    dups_count = row[0] - row[1]
    return dups_count


def _count_nulls(con: duckdb.DuckDBPyConnection, path: Path) -> dict[str, int]:
    """Return {column_name: number of NULLs}."""

    cols = sorted(EXPECTED_COLUMNS)

    parts = ",\n".join(f"COUNT(*) FILTER (WHERE {c} IS NULL)" for c in cols)
    query = f"SELECT {parts} FROM read_parquet('{path}')"
    row = con.execute(query).fetchone()

    if not row:
        raise ValueError("_count_nulls function extracted no data")

    nulls_count = dict(zip(cols, row))

    return nulls_count


def _get_basket_types(con: duckdb.DuckDBPyConnection, path: Path) -> list[str]:
    """Return list of basket types."""

    rows = con.execute(
        f"SELECT DISTINCT basket_type FROM read_parquet('{path}')"
    ).fetchall()

    return [basket_type for (basket_type,) in rows]


def _count_baskets(con: duckdb.DuckDBPyConnection, path: Path) -> dict[str, int]:
    """Return {basket_type: number of baskets} for the file."""

    rows = con.execute(f"""
        SELECT
            basket_type,
            COUNT(DISTINCT basket_id)
        FROM read_parquet('{path}')
        GROUP BY basket_type
    """).fetchall()

    return {basket_type: count for basket_type, count in rows}


if __name__ == "__main__":
    validate_baskets(BASKETS_PARQUET_PATH)
