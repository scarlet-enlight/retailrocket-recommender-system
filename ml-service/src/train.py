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
    """Save mined association rules to a local CSV file in data/processed/."""
    LOCAL_RULES_CSV_PATH.parent.mkdir(parents=True, exist_ok=True)
    df_rules.to_csv(LOCAL_RULES_CSV_PATH, index=False)
    print(f"[Train] Saved {len(df_rules):,} rules locally to: {LOCAL_RULES_CSV_PATH}")


def ensure_referenced_items_exist(conn, item_ids: set[int], is_pascal_case: bool) -> None:
    """
    Ensure all item IDs referenced by recommendation rules exist in historical.items
    so foreign key constraints on ml.recommendation_rules are satisfied.
    """
    if not item_ids:
        return

    if is_pascal_case:
        existing_rows = conn.execute(text('SELECT "ItemId" FROM historical.items')).fetchall()
        existing_ids = {int(r[0]) for r in existing_rows}
        missing_ids = sorted(item_ids - existing_ids)
        if missing_ids:
            # Ensure at least one default category exists in historical.categories
            cat_row = conn.execute(text('SELECT "CategoryId" FROM historical.categories LIMIT 1')).fetchone()
            if cat_row is None:
                conn.execute(text('INSERT INTO historical.categories ("CategoryId", "Name") VALUES (0, \'Default\')'))
                default_cat_id = 0
            else:
                default_cat_id = int(cat_row[0])

            items_df = pd.DataFrame({
                "ItemId": missing_ids,
                "CategoryId": default_cat_id,
                "IsAvailable": True,
            })
            items_df.to_sql("items", conn, schema="historical", if_exists="append", index=False)
            print(f"[Train] Inserted {len(missing_ids):,} missing item IDs into historical.items.")
    else:
        existing_rows = conn.execute(text("SELECT item_id FROM historical.items")).fetchall()
        existing_ids = {int(r[0]) for r in existing_rows}
        missing_ids = sorted(item_ids - existing_ids)
        if missing_ids:
            items_df = pd.DataFrame({
                "item_id": missing_ids,
                "category_id": None,
                "available": True,
            })
            items_df.to_sql("items", conn, schema="historical", if_exists="append", index=False)
            print(f"[Train] Inserted {len(missing_ids):,} missing item IDs into historical.items.")


def save_rules_to_database(df_rules: pd.DataFrame) -> bool:
    """
    Export association rules to the PostgreSQL ml.recommendation_rules table.
    Supports both init.sql (snake_case) and EF Core migration (PascalCase) schemas.
    """
    try:
        engine = create_engine(DATABASE_URL)
        with engine.begin() as conn:
            inspector = inspect(conn)
            columns = [col["name"] for col in inspector.get_columns("recommendation_rules", schema="ml")]
            is_pascal_case = "IfItemId" in columns

            # Collect all unique item IDs appearing in rules
            referenced_items = set(df_rules["if_item_id"]).union(set(df_rules["then_item_id"]))
            ensure_referenced_items_exist(conn, referenced_items, is_pascal_case)

            # Clear previous recommendation rules before inserting the newly trained batch
            conn.execute(text("DELETE FROM ml.recommendation_rules"))

            if is_pascal_case:
                # Match C# Entity Framework Core migration column names
                export_df = pd.DataFrame({
                    "RecommendationRuleId": [uuid.uuid4() for _ in range(len(df_rules))],
                    "IfItemId": df_rules["if_item_id"],
                    "ThenItemId": df_rules["then_item_id"],
                    "Support": df_rules["support"],
                    "Confidence": df_rules["confidence"],
                    "Lift": df_rules["lift"],
                    "CreatedAt": datetime.now(timezone.utc),
                })
            else:
                # Match database/init.sql snake_case column names
                export_df = df_rules[["if_item_id", "then_item_id", "support", "confidence", "lift"]].copy()

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
