"""
Configuration settings for the ML Service pipeline.
Centralizes file paths, Apriori hyperparameters, and PostgreSQL connection settings.
"""

import os
from pathlib import Path
from dotenv import load_dotenv

# Resolve repository root directory (two levels up from ml-service/src/config.py)
PROJECT_ROOT = Path(__file__).resolve().parents[2]

# Load environment variables from database/.env if it exists
ENV_FILE_PATH = PROJECT_ROOT / "database" / ".env"
if ENV_FILE_PATH.exists():
    load_dotenv(dotenv_path=ENV_FILE_PATH)

# Dataset file paths
RAW_EVENTS_PATH = PROJECT_ROOT / "data" / "raw" / "events.csv"
PROCESSED_BASKETS_PATH = PROJECT_ROOT / "data" / "processed" / "baskets.parquet"
LOCAL_RULES_CSV_PATH = PROJECT_ROOT / "data" / "processed" / "recommendation_rules.csv"

# Apriori algorithm hyperparameters (determined in 02_apriori_experiments.ipynb)
BASKET_TYPE = "session_cart"
MIN_SUPPORT = 0.0005     # Itemset must appear in at least 0.05% of multi-item baskets
MIN_CONFIDENCE = 0.10    # At least 10% conditional probability P(B | A)
MIN_LIFT = 1.20          # Positive correlation threshold between antecedent and consequent

# PostgreSQL database configuration
DB_USER = os.getenv("DB_USER", "admin")
DB_PASSWORD = os.getenv("DB_PASSWORD", "admin")
DB_HOST = os.getenv("DB_HOST", "localhost")
DB_PORT = os.getenv("DB_HOST_PORT", "5559")
DB_NAME = os.getenv("DB_NAME", "retailrocket")

DATABASE_URL = f"postgresql+psycopg2://{DB_USER}:{DB_PASSWORD}@{DB_HOST}:{DB_PORT}/{DB_NAME}"
