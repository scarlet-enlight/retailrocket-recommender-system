"""
Core Apriori pipeline functions for loading shopping baskets and mining association rules.
"""

from pathlib import Path
import sys
import warnings
import pandas as pd
from mlxtend.preprocessing import TransactionEncoder
from mlxtend.frequent_patterns import apriori, association_rules

# Ensure ml-service root is on sys.path when executed directly
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

from src.config import (
    PROCESSED_BASKETS_PATH,
    RAW_EVENTS_PATH,
    BASKET_TYPE,
    MIN_SUPPORT,
    MIN_CONFIDENCE,
    MIN_LIFT,
)

VALID_BASKET_TYPES = ("session_cart", "transaction")
REQUIRED_PARQUET_COLUMNS = {"basket_type", "basket_id", "item_id"}


def load_baskets(
    processed_path: Path = PROCESSED_BASKETS_PATH,
    raw_path: Path = RAW_EVENTS_PATH,
    basket_type: str = BASKET_TYPE,
) -> list[list[int]]:
    """
    Load multi-item shopping baskets from the processed baskets.parquet file if available,
    or fall back with a warning to building baskets from raw events.csv.
    """
    if basket_type not in VALID_BASKET_TYPES:
        raise ValueError(
            f"Unknown basket_type '{basket_type}'. Expected one of: {list(VALID_BASKET_TYPES)}"
        )

    if processed_path.exists():
        print(f"[Pipeline] Loading processed baskets from: {processed_path}")
        df = pd.read_parquet(processed_path)

        missing_cols = REQUIRED_PARQUET_COLUMNS - set(df.columns)
        if missing_cols:
            raise ValueError(
                f"Missing required columns in {processed_path.name}: {sorted(missing_cols)}"
            )

        available_types = df["basket_type"].unique().tolist()
        if basket_type not in available_types:
            raise ValueError(
                f"Requested basket_type '{basket_type}' not found in {processed_path.name} "
                f"(available: {available_types})"
            )

        df = df[df["basket_type"] == basket_type].copy()
        df["item_id"] = df["item_id"].astype(int)
        raw_baskets = (
            df.groupby("basket_id")["item_id"].apply(lambda s: list(set(s))).tolist()
        )

    else:
        warning_msg = (
            f"Processed Parquet file not found at {processed_path}. "
            f"Falling back to raw CSV ({raw_path}). "
            "Run the data-pipeline transform stage to generate baskets.parquet."
        )
        print(f"[Pipeline] WARNING: {warning_msg}")
        warnings.warn(warning_msg, UserWarning, stacklevel=2)

        if not raw_path.exists():
            raise FileNotFoundError(
                f"Neither processed Parquet ({processed_path}) nor raw CSV ({raw_path}) exists."
            )

        events = pd.read_csv(raw_path)

        if basket_type == "session_cart":
            # Filter addtocart and transaction events to capture full shopping intent
            cart_events = events[events["event"].isin(["addtocart", "transaction"])].copy()
            cart_events["date"] = pd.to_datetime(cart_events["timestamp"], unit="ms").dt.date
            cart_events["itemid"] = cart_events["itemid"].astype(int)

            # Deduplicate items within the same visitor 24h session
            cart_events = cart_events[["visitorid", "date", "itemid"]].drop_duplicates()
            cart_events["session_id"] = (
                cart_events["visitorid"].astype(str) + "_" + cart_events["date"].astype(str)
            )
            raw_baskets = cart_events.groupby("session_id")["itemid"].apply(list).tolist()
        else:
            tx_events = (
                events[events["event"] == "transaction"]
                .dropna(subset=["transactionid"])
                .copy()
            )
            tx_events["transactionid"] = tx_events["transactionid"].astype(int)
            tx_events["itemid"] = tx_events["itemid"].astype(int)
            tx_events = tx_events[["transactionid", "itemid"]].drop_duplicates()
            raw_baskets = tx_events.groupby("transactionid")["itemid"].apply(list).tolist()

    # Keep only baskets with at least 2 distinct items (single items cannot form pairs)
    multi_item_baskets = [b for b in raw_baskets if len(b) >= 2]
    print(
        f"[Pipeline] Total baskets: {len(raw_baskets):,} | "
        f"Multi-item baskets (>= 2 items): {len(multi_item_baskets):,}"
    )
    return multi_item_baskets


def mine_association_rules(
    baskets: list[list[int]],
    min_support: float = MIN_SUPPORT,
    min_confidence: float = MIN_CONFIDENCE,
    min_lift: float = MIN_LIFT,
) -> pd.DataFrame:
    """
    Run the Apriori algorithm on multi-item baskets and return pairwise (1-to-1)
    recommendation rules formatted for the ml.recommendation_rules database table.
    """
    # 1. One-hot encode baskets into a boolean matrix (rows = baskets, columns = item IDs)
    encoder = TransactionEncoder()
    encoded_array = encoder.fit(baskets).transform(baskets)
    df_encoded = pd.DataFrame(encoded_array, columns=encoder.columns_)
    print(f"[Pipeline] Encoded boolean matrix shape: {df_encoded.shape}")

    # 2. Extract frequent itemsets using Apriori (max_len=2 for pairwise 1-to-1 rules, low_memory=True to avoid large 3D RAM allocations)
    frequent_itemsets = apriori(
        df_encoded,
        min_support=min_support,
        use_colnames=True,
        max_len=2,
        low_memory=True,
    )
    print(f"[Pipeline] Frequent itemsets discovered: {len(frequent_itemsets):,}")

    if frequent_itemsets.empty:
        return pd.DataFrame(columns=["if_item_id", "then_item_id", "support", "confidence", "lift"])

    # 3. Generate association rules filtered by minimum lift and confidence
    rules = association_rules(frequent_itemsets, metric="lift", min_threshold=min_lift)
    rules = rules[rules["confidence"] >= min_confidence].copy()

    # 4. Keep only 1-to-1 pairwise rules (If Item A -> Then Item B) for the database schema
    rules["antecedent_len"] = rules["antecedents"].apply(len)
    rules["consequent_len"] = rules["consequents"].apply(len)
    pairwise_rules = rules[
        (rules["antecedent_len"] == 1) & (rules["consequent_len"] == 1)
    ].copy()

    # 5. Format columns to match PostgreSQL ml.recommendation_rules
    db_rules = pd.DataFrame({
        "if_item_id": pairwise_rules["antecedents"].apply(lambda s: int( next(iter(s)) )),
        "then_item_id": pairwise_rules["consequents"].apply(lambda s: int( next(iter(s)) )),
        "support": pairwise_rules["support"].round(6),
        "confidence": pairwise_rules["confidence"].round(4),
        "lift": pairwise_rules["lift"].round(4),
    })

    # Sort by lift and confidence descending, and remove any duplicate pairs
    db_rules = (
        db_rules[db_rules["if_item_id"] != db_rules["then_item_id"]]
        .drop_duplicates(subset=["if_item_id", "then_item_id"])
        .sort_values(by=["lift", "confidence"], ascending=False)
        .reset_index(drop=True)
    )

    print(f"[Pipeline] Final pairwise recommendation rules: {len(db_rules):,}")
    return db_rules
