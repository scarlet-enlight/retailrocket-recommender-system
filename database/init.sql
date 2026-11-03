-- SCHEMA: historical (DE / Source Data)
CREATE SCHEMA IF NOT EXISTS historical;

CREATE TABLE historical.categories (
    category_id INT PRIMARY KEY,
    parent_id   INT REFERENCES historical.categories(category_id),
    name        VARCHAR(200)
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
    visitor_id  INT NOT NULL REFERENCES historical.visitors(visitor_id),
    item_id     INT NOT NULL REFERENCES historical.items(item_id),
    event_type  VARCHAR(20) NOT NULL CHECK (event_type IN ('view', 'addtocart', 'transaction')),
    ts          BIGINT NOT NULL
);

CREATE TABLE historical.transactions (
    transaction_id INT PRIMARY KEY,
    visitor_id     INT NOT NULL REFERENCES historical.visitors(visitor_id),
    ts             BIGINT NOT NULL
);

CREATE TABLE historical.transaction_items (
    transaction_id INT REFERENCES historical.transactions(transaction_id),
    item_id        INT REFERENCES historical.items(item_id),
    PRIMARY KEY (transaction_id, item_id)
);


-- SCHEMA: shop (Web Development / Operational Application)
CREATE SCHEMA IF NOT EXISTS shop;

CREATE TABLE shop.users (
    user_id       UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username      VARCHAR(100) UNIQUE NOT NULL,
    email         VARCHAR(200) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE shop.products (
    product_id  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    item_id     INT NOT NULL REFERENCES historical.items(item_id),
    name        VARCHAR(200),
    price       DECIMAL(10,2),
    category_id INT NOT NULL REFERENCES historical.categories(category_id)
);

CREATE TABLE shop.cart (
    cart_id    UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID NOT NULL REFERENCES shop.users(user_id),
    product_id UUID NOT NULL REFERENCES shop.products(product_id),
    quantity   INT NOT NULL DEFAULT 1 CHECK (quantity > 0),
    added_at   TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE shop.orders (
    order_id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID NOT NULL REFERENCES shop.users(user_id),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    total      DECIMAL(10,2) NOT NULL
);

CREATE TABLE shop.order_items (
    order_id    UUID REFERENCES shop.orders(order_id),
    product_id  UUID REFERENCES shop.products(product_id),
    quantity    INT NOT NULL,
    price       DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (order_id, product_id)
);

-- SCHEMA: ml (Data Science / Analytics)
CREATE SCHEMA IF NOT EXISTS ml;

CREATE TABLE ml.recommendation_rules (
    rule_id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    if_item_id     INT NOT NULL REFERENCES historical.items(item_id),
    then_item_id   INT NOT NULL REFERENCES historical.items(item_id),
    support        FLOAT NOT NULL,
    confidence     FLOAT NOT NULL,
    lift           FLOAT NOT NULL,
    created_at     TIMESTAMP NOT NULL DEFAULT NOW()
);
