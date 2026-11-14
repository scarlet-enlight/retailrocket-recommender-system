"""
Evaluation script for the Apriori product recommendation pipeline.
Measures rule quality distributions, catalog coverage, and offline ranking
metrics (Hit Rate@K, Precision@K, Recall@K) against a popularity baseline.
"""

from collections import Counter
from pathlib import Path
import random
import sys
import pandas as pd

# Ensure ml-service root is on sys.path when executed directly
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

from src.config import (
    EVALUATION_METRICS_CSV_PATH,
    MIN_CONFIDENCE,
    MIN_LIFT,
    MIN_SUPPORT,
)
from src.pipeline import load_baskets, mine_association_rules


# Data splitting and lookup helpers

def split_baskets(
    baskets: list[list[int]],
    test_ratio: float = 0.20,
    seed: int = 42,
) -> tuple[list[list[int]], list[list[int]]]:
    """Split multi-item baskets deterministically into training and test subsets."""
    rng = random.Random(seed)
    shuffled = list(baskets)
    rng.shuffle(shuffled)

    split_idx = int(len(shuffled) * (1.0 - test_ratio))
    train_baskets = shuffled[:split_idx]
    test_baskets = shuffled[split_idx:]
    return train_baskets, test_baskets


def build_rule_lookup(df_rules: pd.DataFrame) -> dict[int, list[int]]:
    """
    Build a dictionary mapping each trigger item (if_item_id) to an ordered list
    of recommended target items (then_item_id), sorted by lift and confidence.
    """
    if df_rules.empty:
        return {}

    sorted_rules = df_rules.sort_values(by=["lift", "confidence"], ascending=False)
    lookup: dict[int, list[int]] = {}
    for row in sorted_rules.itertuples(index=False):
        antecedent = int(row.if_item_id)
        consequent = int(row.then_item_id)
        lookup.setdefault(antecedent, []).append(consequent)
    return lookup


def get_popular_items(baskets: list[list[int]], top_n: int = 50) -> list[int]:
    """Return the most frequent items across training baskets for baseline comparison."""
    counts: Counter[int] = Counter()
    for basket in baskets:
        counts.update(set(basket))
    return [item_id for item_id, _ in counts.most_common(top_n)]


# Intrinsic rule quality and coverage statistics

def compute_rule_statistics(
    df_rules: pd.DataFrame,
    baskets: list[list[int]],
) -> dict[str, float | int]:
    """
    Calculate summary metrics describing the mined association rules,
    including support/confidence/lift distributions and item coverage.
    """
    catalog_items = {item for basket in baskets for item in basket}

    if df_rules.empty:
        return {
            "rules_count": 0,
            "unique_antecedents": 0,
            "unique_consequents": 0,
            "unique_rule_items": 0,
            "catalog_items": len(catalog_items),
            "item_coverage_pct": 0.0,
            "basket_coverage_pct": 0.0,
            "avg_rules_per_item": 0.0,
            "mean_support": 0.0,
            "median_support": 0.0,
            "mean_confidence": 0.0,
            "median_confidence": 0.0,
            "mean_lift": 0.0,
            "median_lift": 0.0,
            "max_lift": 0.0,
        }

    antecedents = set(df_rules["if_item_id"].astype(int))
    consequents = set(df_rules["then_item_id"].astype(int))
    rule_items = antecedents.union(consequents)

    covered_baskets = sum(
        1 for basket in baskets if any(item in antecedents for item in basket)
    )
    rules_per_item = df_rules.groupby("if_item_id")["then_item_id"].count()

    return {
        "rules_count": int(len(df_rules)),
        "unique_antecedents": len(antecedents),
        "unique_consequents": len(consequents),
        "unique_rule_items": len(rule_items),
        "catalog_items": len(catalog_items),
        "item_coverage_pct": round(len(rule_items) / max(len(catalog_items), 1) * 100, 2),
        "basket_coverage_pct": round(covered_baskets / max(len(baskets), 1) * 100, 2),
        "avg_rules_per_item": round(float(rules_per_item.mean()), 2),
        "mean_support": round(float(df_rules["support"].mean()), 6),
        "median_support": round(float(df_rules["support"].median()), 6),
        "mean_confidence": round(float(df_rules["confidence"].mean()), 4),
        "median_confidence": round(float(df_rules["confidence"].median()), 4),
        "mean_lift": round(float(df_rules["lift"].mean()), 2),
        "median_lift": round(float(df_rules["lift"].median()), 2),
        "max_lift": round(float(df_rules["lift"].max()), 2),
    }


# Offline given-one ranking evaluation

def _score_predictions(
    recommendations: list[int],
    ground_truth: set[int],
    k: int,
) -> tuple[int, float, float]:
    """Compute Hit@K, Precision@K, and Recall@K for a single recommendation list."""
    top_k = recommendations[:k]
    if not top_k or not ground_truth:
        return 0, 0.0, 0.0

    hits = sum(1 for item in top_k if item in ground_truth)
    hit_flag = 1 if hits > 0 else 0
    precision = hits / k
    recall = hits / len(ground_truth)
    return hit_flag, precision, recall


def evaluate_recommendations_at_k(
    test_baskets: list[list[int]],
    rule_lookup: dict[int, list[int]],
    popular_items: list[int],
    k_values: list[int] | None = None,
) -> pd.DataFrame:
    """
    Evaluate top-K recommendation quality on held-out test baskets using a
    given-one protocol: given a single trigger item x in basket B, treat the
    remaining basket items B \\ {x} as ground truth.
    """
    if k_values is None:
        k_values = [1, 3, 5, 10]

    records = []

    for k in k_values:
        apriori_cov_hits, apriori_cov_prec, apriori_cov_rec = [], [], []
        pop_cov_hits, pop_cov_prec, pop_cov_rec = [], [], []
        hybrid_hits, hybrid_prec, hybrid_rec = [], [], []
        pop_hits, pop_prec, pop_rec = [], [], []

        for basket in test_baskets:
            unique_items = list(set(basket))
            if len(unique_items) < 2:
                continue

            for trigger_item in unique_items:
                ground_truth = set(unique_items) - {trigger_item}
                apriori_recs = rule_lookup.get(trigger_item, [])
                pop_recs = [item for item in popular_items if item != trigger_item][:k]

                # 1. Covered trigger items (warm-start queries): Apriori vs Popularity on identical queries
                if apriori_recs:
                    h, p, r = _score_predictions(apriori_recs, ground_truth, k)
                    apriori_cov_hits.append(h)
                    apriori_cov_prec.append(p)
                    apriori_cov_rec.append(r)

                    h_pc, p_pc, r_pc = _score_predictions(pop_recs, ground_truth, k)
                    pop_cov_hits.append(h_pc)
                    pop_cov_prec.append(p_pc)
                    pop_cov_rec.append(r_pc)

                # 2. Hybrid across all queries: Apriori rules first, padded with popular items up to K
                seen = set(apriori_recs) | {trigger_item}
                hybrid_recs = list(apriori_recs[:k])
                if len(hybrid_recs) < k:
                    for item in popular_items:
                        if item not in seen:
                            hybrid_recs.append(item)
                            if len(hybrid_recs) == k:
                                break
                h_hyb, p_hyb, r_hyb = _score_predictions(hybrid_recs, ground_truth, k)
                hybrid_hits.append(h_hyb)
                hybrid_prec.append(p_hyb)
                hybrid_rec.append(r_hyb)

                # 3. Popularity Baseline across all queries
                h_pop, p_pop, r_pop = _score_predictions(pop_recs, ground_truth, k)
                pop_hits.append(h_pop)
                pop_prec.append(p_pop)
                pop_rec.append(r_pop)

        records.append({
            "model": "Apriori (Covered Items)",
            "k": k,
            "queries_evaluated": len(apriori_cov_hits),
            "hit_rate_pct": round(sum(apriori_cov_hits) / max(len(apriori_cov_hits), 1) * 100, 2),
            "precision_pct": round(sum(apriori_cov_prec) / max(len(apriori_cov_prec), 1) * 100, 2),
            "recall_pct": round(sum(apriori_cov_rec) / max(len(apriori_cov_rec), 1) * 100, 2),
        })
        records.append({
            "model": "Popularity Baseline (Covered Items)",
            "k": k,
            "queries_evaluated": len(pop_cov_hits),
            "hit_rate_pct": round(sum(pop_cov_hits) / max(len(pop_cov_hits), 1) * 100, 2),
            "precision_pct": round(sum(pop_cov_prec) / max(len(pop_cov_prec), 1) * 100, 2),
            "recall_pct": round(sum(pop_cov_rec) / max(len(pop_cov_rec), 1) * 100, 2),
        })
        records.append({
            "model": "Apriori + Popularity (Hybrid)",
            "k": k,
            "queries_evaluated": len(hybrid_hits),
            "hit_rate_pct": round(sum(hybrid_hits) / max(len(hybrid_hits), 1) * 100, 2),
            "precision_pct": round(sum(hybrid_prec) / max(len(hybrid_prec), 1) * 100, 2),
            "recall_pct": round(sum(hybrid_rec) / max(len(hybrid_rec), 1) * 100, 2),
        })
        records.append({
            "model": "Popularity Baseline (All Queries)",
            "k": k,
            "queries_evaluated": len(pop_hits),
            "hit_rate_pct": round(sum(pop_hits) / max(len(pop_hits), 1) * 100, 2),
            "precision_pct": round(sum(pop_prec) / max(len(pop_prec), 1) * 100, 2),
            "recall_pct": round(sum(pop_rec) / max(len(pop_rec), 1) * 100, 2),
        })

    return pd.DataFrame(records)


def run_evaluation() -> tuple[dict[str, float | int], pd.DataFrame]:
    """Execute the full model evaluation workflow and save metrics to CSV."""
    print("Starting Retailrocket Apriori Model Evaluation")

    baskets = load_baskets()
    full_rules = mine_association_rules(
        baskets,
        min_support=MIN_SUPPORT,
        min_confidence=MIN_CONFIDENCE,
        min_lift=MIN_LIFT,
    )
    stats = compute_rule_statistics(full_rules, baskets)

    print("\nProduction Rule Quality & Coverage Summary:")
    for key, value in stats.items():
        print(f"  {key}: {value}")

    train_baskets, test_baskets = split_baskets(baskets, test_ratio=0.20, seed=42)
    print(f"\nTrain/Test Split: {len(train_baskets):,} train baskets | {len(test_baskets):,} test baskets")

    train_rules = mine_association_rules(
        train_baskets,
        min_support=MIN_SUPPORT,
        min_confidence=MIN_CONFIDENCE,
        min_lift=MIN_LIFT,
    )
    rule_lookup = build_rule_lookup(train_rules)
    popular_items = get_popular_items(train_baskets, top_n=50)

    df_eval = evaluate_recommendations_at_k(
        test_baskets=test_baskets,
        rule_lookup=rule_lookup,
        popular_items=popular_items,
        k_values=[1, 3, 5, 10],
    )

    EVALUATION_METRICS_CSV_PATH.parent.mkdir(parents=True, exist_ok=True)
    df_eval.to_csv(EVALUATION_METRICS_CSV_PATH, index=False)

    print("\nOffline Ranking Evaluation Results:")
    print(df_eval.to_string(index=False))
    print(f"\nSaved evaluation metrics to: {EVALUATION_METRICS_CSV_PATH}")

    return stats, df_eval


if __name__ == "__main__":
    run_evaluation()
