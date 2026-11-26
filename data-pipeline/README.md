# data-pipeline

ELT pipeline that prepares the Retailrocket data for the recommendation system.

| Layer | Location | Content |
| :--- | :--- | :--- |
| Bronze | `data/raw/` | Original CSV files |
| Silver | PostgreSQL schema `historical` | Typed tables loaded from the CSV files |
| Gold | `data/processed/baskets.parquet` | Baskets used by the ML service |

## Stages

1. **extract** (`extract.py`): reads the CSV files with pandas using explicit column types. From the `item_properties` files only the `categoryid` and `available` properties are kept. A missing or empty file stops the pipeline with an error.
2. **load** (`load.py`): loads the data into the `historical` tables (`categories`, `visitors`, `items`, `events`, `transactions`, `transaction_items`) with `COPY`, in one transaction. Running it again truncates and reloads the tables, so the load is idempotent. It refuses to run if dependent tables (`ml.recommendation_rules`, `shop.products`) contain data.
3. **transform** (`transform.py`): DuckDB attaches PostgreSQL in read-only mode and exports the baskets to Parquet. The file is written to a temporary path first and then renamed, so a failed run never leaves a broken file.
4. **validate** (`utils/validators.py`): checks columns, types, NULLs, duplicates, basket types and minimum basket counts. On a failure the pipeline stops with a `DataQualityError` and a non-zero exit code.

## Basket contract

| Column | Type | Meaning |
| :--- | :--- | :--- |
| `basket_type` | VARCHAR | `session_cart` or `transaction` |
| `basket_id` | VARCHAR | `<visitor_id>_<UTC date>` for session carts, the transaction id for transactions |
| `item_id` | INTEGER | Item in the basket (never NULL) |

## Requirements

- Python 3.12 and [uv](https://docs.astral.sh/uv/)
- Docker (for PostgreSQL)
- CSV files in `data/raw/`: `events.csv`, `item_properties_part1.csv`, `item_properties_part2.csv`, `category_tree.csv`
- `database/.env` created from `database/.env.example`. `DB_PASSWORD` has to be set; the other values default to `localhost:5559`, database `retailrocket`, user `admin`.

## Run

Start PostgreSQL first (the schema is created from `database/init.sql`):

```bash
cd database
cp .env.example .env
docker compose up -d
```

Then run the pipeline:

```bash
cd data-pipeline
uv sync
uv run data-pipeline                  # load + transform
uv run data-pipeline --only load
uv run data-pipeline --only transform
```

The exit code is `0` on success and `1` on any failure.

## Expected result

`data/processed/baskets.parquet` has 88,695 rows: 43,265 `session_cart` baskets and 17,672 `transaction` baskets.

## Reloading from scratch

Recommended (deletes all data in the database):

```bash
cd database
docker compose down -v
docker compose up -d
```

Then run the pipeline again and retrain the ML service (`uv run python src/train.py` in `ml-service`).

If you only want to reload `historical` without recreating the database, clear the dependent tables first, otherwise `load` stops with an error. Note that this also removes carts and order items, because they reference the products:

```bash
docker exec -it retailrocker-postgres psql -U <DB_USER> -d <DB_NAME> \
  -c "TRUNCATE ml.recommendation_rules, shop.products CASCADE;"
```

## Tests and CI

```bash
uv run pytest
uv run ruff check .
uv run ruff format --check .
```

The same checks run on every pull request (GitHub Actions, `.github/workflows/ci.yml`).