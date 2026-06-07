-- SCHEMA: historical (DE / Source Data)
CREATE SCHEMA IF NOT EXISTS historical;

CREATE TABLE historical.categories (
    category_id INT PRIMARY KEY,
    parent_id INT REFERENCES historical.categories(category_id)
);

CREATE TABLE historical.items (
    item_id     INT PRIMARY KEY,
    category_id INT REFERENCES historical.categories(category_id),
    available   BOOLEAN
);

CREATE TABLE historical.visitors (
    visitor_id INT PRIMARY KEY
);

CREATE TABLE historical.events (
    event_id    SERIAL PRIMARY KEY,
    visitor_id  INT REFERENCES historical.visitors(visitor_id),
    item_id     INT REFERENCES historical.items(item_id),
    event_type VARCHAR(20) CHECK (event_type IN ('view', 'addtocart', 'transaction')),
    ts BIGINT
);

CREATE TABLE historical.transactions (
    transaction_id INT PRIMARY KEY,
    visitor_id     INT REFERENCES historical.visitors(visitor_id),
    ts             BIGINT
);

CREATE TABLE historical.transaction_items (
    transaction_id INT REFERENCES historical.transactions(transaction_id),
    item_id        INT REFERENCES historical.items(item_id),
    PRIMARY KEY (transaction_id, item_id)
);


-- SCHEMA: shop (Web Development / Operational Application)
CREATE SCHEMA IF NOT EXISTS shop;

CREATE TABLE shop.users (
    user_id       SERIAL PRIMARY KEY,
    username      VARCHAR(100) UNIQUE NOT NULL,
    email         VARCHAR(200) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMP DEFAULT NOW()
);

CREATE TABLE shop.products (
    product_id  INT PRIMARY KEY,
    item_id     INT REFERENCES historical.items(item_id),
    name        VARCHAR(200),
    price       DECIMAL(10,2),
    category_id INT REFERENCES historical.categories(category_id)
);

CREATE TABLE shop.cart (
    cart_id    SERIAL PRIMARY KEY,
    user_id    INT REFERENCES shop.users(user_id),
    product_id INT REFERENCES shop.products(product_id),
    quantity   INT DEFAULT 1,
    added_at   TIMESTAMP DEFAULT NOW()
);

CREATE TABLE shop.orders (
    order_id   SERIAL PRIMARY KEY,
    user_id    INT REFERENCES shop.users(user_id),
    created_at TIMESTAMP DEFAULT NOW(),
    total      DECIMAL(10,2)
);

CREATE TABLE shop.order_items (
    order_id   INT REFERENCES shop.orders(order_id),
    product_id INT REFERENCES shop.products(product_id),
    quantity   INT,
    price      DECIMAL(10,2),
    PRIMARY KEY (order_id, product_id)
);

-- SCHEMA: ml (Data Science / Analytics)
CREATE SCHEMA IF NOT EXISTS ml;

CREATE TABLE ml.recommendation_rules (
    rule_id        SERIAL PRIMARY KEY,
    if_item_id     INT REFERENCES historical.items(item_id),
    then_item_id   INT REFERENCES historical.items(item_id),
    support        FLOAT,
    confidence     FLOAT,
    lift           FLOAT,
    created_at     TIMESTAMP DEFAULT NOW()
);
