import os
from dataclasses import dataclass
from pathlib import Path

from dotenv import load_dotenv

load_dotenv()


@dataclass(frozen=True)
class DBConfig:
    host: str
    port: int
    name: str
    user: str
    password: str

    @property
    def connection_string(self) -> str:
        return f"postgresql://{self.user}:{self.password}@{self.host}:{self.port}/{self.name}"


# Database config
DB_HOST = os.getenv("DB_HOST", "localhost")
DB_PORT = int(os.getenv("DB_PORT", 5559))
DB_NAME = os.getenv("DB_NAME", "retailrocket")
DB_USER = os.getenv("DB_USER", "admin")
DB_PASSWORD = os.getenv("DB_PASSWORD", "admin")

db_config = DBConfig(DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASSWORD)


# Paths
ROOT_DIR = Path(__file__).parent.parent.parent.parent

RAW_DATA_DIR = ROOT_DIR / "data" / "raw"
PROCESSED_DATA_DIR = ROOT_DIR / "data" / "processed"

# DuckDB
DUCKDB_PATH = PROCESSED_DATA_DIR / "retailrocket.duckdb"
BASKETS_PARQUET_PATH = PROCESSED_DATA_DIR / "baskets.parquet"

# CSV files
EVENTS_PATH = RAW_DATA_DIR / "events.csv"
ITEM_PROPERTIES_1_PATH = RAW_DATA_DIR / "item_properties_part1.csv"
ITEM_PROPERTIES_2_PATH = RAW_DATA_DIR / "item_properties_part2.csv"
CATEGORY_TREE_PATH = RAW_DATA_DIR / "category_tree.csv"
