import argparse
import sys
import time
from collections.abc import Callable
from typing import Any

import psycopg2

from data_pipeline.config import BASKETS_PARQUET_PATH, db_config
from data_pipeline.extract import extract_all
from data_pipeline.load import load_historical
from data_pipeline.transform import run_transform
from data_pipeline.utils.logger import setup_logger

logger = setup_logger(name="pipeline_logger")


def run_stage(name: str, step: Callable[..., Any], *args) -> Any:
    """Run one stage with timing and uniform logging."""

    logger.info("Stage %s started", name)
    started = time.perf_counter()
    result = step(*args)
    logger.info("Stage %s finished in %.1fs", name, time.perf_counter() - started)

    return result


def run_load() -> None:
    """Extract data from csv and load into postgres database."""

    data = run_stage("extract", extract_all)
    conn = psycopg2.connect(db_config.connection_string)

    try:
        run_stage("load", load_historical, conn, data)
    finally:
        conn.close()


def _parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()

    parser.add_argument(
        "--only",
        choices=["load", "transform"],
        default=None,
        help="Run only listed step (by default: all steps)",
    )

    return parser.parse_args()


def main() -> int:
    args = _parse_args()

    try:
        if args.only in (None, "load"):
            run_load()
        if args.only in (None, "transform"):
            run_stage(
                "transform",
                run_transform,
                db_config.connection_string,
                BASKETS_PARQUET_PATH,
            )
    except Exception:
        logger.exception("Pipeline failed")
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
