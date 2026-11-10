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


def extract_events(path: Path) -> pd.DataFrame | None:
    if not path.is_file():
        extract_logger.error(f"File {path} doesn't exist")
        return None

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

    mb = df.memory_usage(deep=True).sum() / 1024**2
    extract_logger.info(f"Extracted {len(df)} rows ({mb:.1f} MB)")

    return df


def extract_categories(path: Path) -> pd.DataFrame | None:
    if not path.is_file():
        extract_logger.error(f"File {path} doesn't exist")
        return None

    extract_logger.info("Started extracting categories...")

    df = pd.read_csv(
        path,
        dtype={
            "categoryid": "int32",
            "parentid": "Int32",
        },
        header=0,
    )

    mb = df.memory_usage(deep=True).sum() / 1024**2
    extract_logger.info(f"Extracted {len(df)} rows ({mb:.1f} MB)")

    return df


def extract_items(path: Path) -> pd.DataFrame | None:
    if not path.is_file():
        extract_logger.error(f"File {path} doesn't exist")
        return None

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

    df = pd.concat(chunks, ignore_index=True)

    mb = df.memory_usage(deep=True).sum() / 1024**2
    extract_logger.info(f"Extracted {len(df)} rows ({mb:.1f} MB)")

    return df


def extract_all_items() -> pd.DataFrame | None:
    parts = [
        extract_items(ITEM_PROPERTIES_1_PATH),
        extract_items(ITEM_PROPERTIES_2_PATH),
    ]

    parts = [p for p in parts if p is not None]
    if not parts:
        return None

    combined = pd.concat(parts, ignore_index=True)
    extract_logger.info(f"Unique items with properties: {combined['itemid'].nunique()}")
    return combined


def extract_all() -> dict[str, pd.DataFrame | None]:
    return {
        "events": extract_events(EVENTS_PATH),
        "categories": extract_categories(CATEGORY_TREE_PATH),
        "items": extract_all_items(),
    }


if __name__ == "__main__":
    extracted_data: dict[str, pd.DataFrame | None] = extract_all()
