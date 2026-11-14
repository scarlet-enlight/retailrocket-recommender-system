from pathlib import Path

import pandas as pd

from data_pipeline.config import (
    CATEGORY_TREE_PATH,
    EVENTS_PATH,
    ITEM_PROPERTIES_1_PATH,
    ITEM_PROPERTIES_2_PATH,
)
from data_pipeline.utils.logger import setup_logger

extract_logger = setup_logger(name="extract_logger")


def extract_events(path: Path) -> pd.DataFrame:
    """Read events.csv."""

    if not path.is_file():
        raise FileNotFoundError(f"events file not found: {path}")

    extract_logger.info("Started extracting events...")

    df = pd.read_csv(
        path,
        dtype={
            "timestamp": "int64",
            "visitorid": "int64",
            "event": "str",
            "itemid": "int64",
            "transactionid": "float64",
        },
        header=0,
    )
    if df.empty:
        raise ValueError(f"{path.name} is empty")

    mb = df.memory_usage(deep=True).sum() / 1024**2
    extract_logger.info(f"Extracted {len(df)} rows ({mb:.1f} MB)")

    return df


def extract_categories(path: Path) -> pd.DataFrame:
    """Read category_tree.csv; parentid is nullable (root categories)."""

    if not path.is_file():
        raise FileNotFoundError(f"categories file not found: {path}")

    extract_logger.info("Started extracting categories...")

    df = pd.read_csv(
        path,
        dtype={
            "categoryid": "int32",
            "parentid": "Int32",
        },
        header=0,
    )
    if df.empty:
        raise ValueError(f"{path.name} is empty")

    mb = df.memory_usage(deep=True).sum() / 1024**2
    extract_logger.info(f"Extracted {len(df)} rows ({mb:.1f} MB)")

    return df


def extract_items(path: Path) -> pd.DataFrame:
    """Read one item_properties part in chunks, keeping only categoryid and available.

    Does not deduplicate: older versions of a property are kept (latest per
    itemid/property is picked later, in load or transform).
    """

    if not path.is_file():
        raise FileNotFoundError(f"items file not found: {path}")

    extract_logger.info("Started extracting items...")

    chunks = []
    for chunk in pd.read_csv(
        path,
        dtype={
            "timestamp": "int64",
            "itemid": "int64",
            "property": "str",
            "value": "str",
        },
        chunksize=500_000,
    ):
        chunks.append(chunk[chunk["property"].isin(["categoryid", "available"])])

    if not chunks:
        raise ValueError(f"{path.name} has no data rows")

    df = pd.concat(chunks, ignore_index=True)
    if df.empty:
        raise ValueError(f"{path.name}: no categoryid/available rows found")

    mb = df.memory_usage(deep=True).sum() / 1024**2
    extract_logger.info(f"Extracted {len(df)} rows ({mb:.1f} MB)")

    return df


def extract_all_items() -> pd.DataFrame:
    """Merge both item_properties parts into one DataFrame."""

    parts = [
        extract_items(ITEM_PROPERTIES_1_PATH),
        extract_items(ITEM_PROPERTIES_2_PATH),
    ]

    combined = pd.concat(parts, ignore_index=True)
    extract_logger.info(f"Unique items with properties: {combined['itemid'].nunique()}")
    return combined


def extract_all() -> dict[str, pd.DataFrame]:
    """Extract all source files into DataFrames.

    Raises FileNotFoundError or ValueError if any file is missing or empty.
    """
    return {
        "events": extract_events(EVENTS_PATH),
        "categories": extract_categories(CATEGORY_TREE_PATH),
        "items": extract_all_items(),
    }


if __name__ == "__main__":
    extracted_data: dict[str, pd.DataFrame] = extract_all()
