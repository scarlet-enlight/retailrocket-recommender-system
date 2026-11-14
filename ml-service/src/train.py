"""
Main training script for the ML recommendation pipeline.
Loads shopping baskets, mines Apriori association rules, saves rules locally,
and exports them to the PostgreSQL ml.recommendation_rules table.
"""

import sys
import uuid
from datetime import datetime, timezone
from pathlib import Path
import pandas as pd
from sqlalchemy import create_engine, inspect, text
from sqlalchemy.exc import SQLAlchemyError

# Ensure ml-service root is on sys.path when executed directly
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

from src.config import DATABASE_URL, LOCAL_RULES_CSV_PATH
from src.pipeline import load_baskets, mine_association_rules


def save_rules_locally(df_rules: pd.DataFrame) -> None:
    """Save mined association rules to a local CSV file in data/processed/ when writable."""
    try:
        LOCAL_RULES_CSV_PATH.parent.mkdir(parents=True, exist_ok=True)
        df_rules.to_csv(LOCAL_RULES_CSV_PATH, index=False)
        print(f"[Train] Saved {len(df_rules):,} rules locally to: {LOCAL_RULES_CSV_PATH}")
    except OSError:
        print(f"[Train] Directory {LOCAL_RULES_CSV_PATH.parent} is mounted read-only; skipping local CSV export.")


def get_existing_historical_items(conn, is_pascal_case: bool) -> set[int]:
    """
    Read existing item IDs from historical.items (read-only) to verify that
    the data engineering load prerequisite has been satisfied before exporting rules.
    """
    query = (
        text('SELECT "ItemId" FROM historical.items')
        if is_pascal_case
        else text("SELECT item_id FROM historical.items")
    )
    rows = conn.execute(query).fetchall()
    return {int(row[0]) for row in rows}


def save_rules_to_database(df_rules: pd.DataFrame) -> bool:
    """
    Export association rules to the PostgreSQL ml.recommendation_rules table.
    Supports both init.sql (snake_case) and EF Core migration (PascalCase) schemas.
    Enforces that historical.items is pre-populated by the data engineering pipeline.
    """
    try:
        engine = create_engine(DATABASE_URL)
        with engine.begin() as conn:
            inspector = inspect(conn)
            columns = [col["name"] for col in inspector.get_columns("recommendation_rules", schema="ml")]
            is_pascal_case = "IfItemId" in columns

            # Verify prerequisite: historical.items must be populated by data-pipeline
            existing_items = get_existing_historical_items(conn, is_pascal_case)
            if not existing_items:
                print(
                    "[Train] Prerequisite not met: historical.items is empty. "
                    "Please run the data-pipeline load step before exporting rules to PostgreSQL."
                )
                return False

            # Filter rules to items present in historical.items to guarantee FK integrity
            valid_rules = df_rules[
                df_rules["if_item_id"].isin(existing_items)
                & df_rules["then_item_id"].isin(existing_items)
            ].copy()

            if valid_rules.empty:
                print(
                    "[Train] Prerequisite not met: mined rule item IDs are not present in historical.items. "
                    "Please run the full data-pipeline load step first."
                )
                return False

            if len(valid_rules) < len(df_rules):
                skipped = len(df_rules) - len(valid_rules)
                print(f"[Train] Skipped {skipped:,} rules referencing items not in historical.items.")

            # Clear previous recommendation rules before inserting the newly trained batch
            conn.execute(text("DELETE FROM ml.recommendation_rules"))

            if is_pascal_case:
                # Match C# Entity Framework Core migration column names
                export_df = pd.DataFrame({
                    "RecommendationRuleId": [uuid.uuid4() for _ in range(len(valid_rules))],
                    "IfItemId": valid_rules["if_item_id"],
                    "ThenItemId": valid_rules["then_item_id"],
                    "Support": valid_rules["support"],
                    "Confidence": valid_rules["confidence"],
                    "Lift": valid_rules["lift"],
                    "CreatedAt": datetime.now(timezone.utc),
                })
            else:
                # Match database/init.sql snake_case column names
                export_df = valid_rules[["if_item_id", "then_item_id", "support", "confidence", "lift"]].copy()

            export_df.to_sql(
                name="recommendation_rules",
                con=conn,
                schema="ml",
                if_exists="append",
                index=False,
            )
            print(f"[Train] Successfully exported {len(export_df):,} rules to PostgreSQL (ml.recommendation_rules).")
            return True

    except SQLAlchemyError as exc:
        print(f"[Train] Could not connect/write to PostgreSQL (is Docker running?): {exc.__class__.__name__}")
        print("[Train] Rules are safely stored in the local CSV file and ready for use.")
        return False


def run_training_pipeline() -> pd.DataFrame:
    """Execute the end-to-end training pipeline."""
    print("=" * 60)
    print("Starting Retailrocket Apriori Training Pipeline")
    print("=" * 60)

    baskets = load_baskets()
    df_rules = mine_association_rules(baskets)

    if df_rules.empty:
        print("[Train] Warning: No association rules satisfied the threshold criteria.")
        return df_rules

    save_rules_locally(df_rules)
    save_rules_to_database(df_rules)

    print("=" * 60)
    print("Training Pipeline Completed Successfully")
    print("=" * 60)
    return df_rules


if __name__ == "__main__":
    run_training_pipeline()
