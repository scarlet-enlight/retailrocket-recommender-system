import os
from dataclasses import dataclass, field
from pathlib import Path
from urllib.parse import quote_plus

from dotenv import load_dotenv

ROOT_DIR = Path(__file__).resolve().parents[3]  # repo root
load_dotenv(ROOT_DIR / "database" / ".env")  # shared with ml-service and Docker


@dataclass(frozen=True)
class DBConfig:
    host: str
    port: int
    name: str
    user: str
    password: str = field(repr=False)  # keeps the password out of print()/logs

    @property
    def connection_string(self) -> str:
        user = quote_plus(self.user)
        password = quote_plus(self.password)  # escapes special characters
        return f"postgresql://{user}:{password}@{self.host}:{self.port}/{self.name}"


def _require_env(name: str) -> str:
    value = os.getenv(name)
    if not value:
        raise RuntimeError(f"{name} is not set (see database/.env.example)")
    return value


db_config = DBConfig(
    host=os.getenv("DB_HOST", "localhost"),
    port=int(os.getenv("DB_HOST_PORT", "5559")),
    name=os.getenv("DB_NAME", "retailrocket"),
    user=os.getenv("DB_USER", "admin"),
    password=_require_env("DB_PASSWORD"),
)

# Paths
RAW_DATA_DIR = ROOT_DIR / "data" / "raw"
PROCESSED_DATA_DIR = ROOT_DIR / "data" / "processed"

# Outpu
BASKETS_PARQUET_PATH = PROCESSED_DATA_DIR / "baskets.parquet"

# CSV files
EVENTS_PATH = RAW_DATA_DIR / "events.csv"
ITEM_PROPERTIES_1_PATH = RAW_DATA_DIR / "item_properties_part1.csv"
ITEM_PROPERTIES_2_PATH = RAW_DATA_DIR / "item_properties_part2.csv"
CATEGORY_TREE_PATH = RAW_DATA_DIR / "category_tree.csv"
