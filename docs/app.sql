-- TradingCenter PostgreSQL Database Schema DDL

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 1. Users Table
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    name VARCHAR(255) NOT NULL,
    base_currency VARCHAR(10) NOT NULL DEFAULT 'BRL',
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Accounts Table (Custody / Brokers / Institutions)
CREATE TABLE accounts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    institution VARCHAR(255) NOT NULL,
    account_type VARCHAR(50) NOT NULL, -- Personal, Joint, Retirement_FGTS, Brokerage
    base_currency VARCHAR(10) NOT NULL DEFAULT 'BRL',
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_accounts_user ON accounts(user_id);

-- 3. Master Assets Catalog Table
CREATE TABLE assets (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    ticker VARCHAR(50) UNIQUE, -- Nullable for non-ticker assets like FGTS, US Private Bonds
    asset_category VARCHAR(50) NOT NULL,
    valuation_type VARCHAR(50) NOT NULL, -- TickerMarket, IndexLinked, FixedRate, ManualBalance
    currency VARCHAR(10) NOT NULL DEFAULT 'BRL',
    index_benchmark VARCHAR(50), -- CDI, IPCA, TR, SELIC, SP500, IBOVESPA, None
    logo_url VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_assets_ticker ON assets(ticker);

-- 4. Investments (User Account Holdings) Table
CREATE TABLE investments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
    asset_id UUID NOT NULL REFERENCES assets(id) ON DELETE CASCADE,
    custom_name VARCHAR(255),
    interest_rate NUMERIC(18, 6), -- e.g. 1.20 for 120% CDI or 0.05 for 5%
    maturity_date TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_investments_account ON investments(account_id);
CREATE INDEX idx_investments_asset ON investments(asset_id);

-- 4. Transactions Table
CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    investment_id UUID NOT NULL REFERENCES investments(id) ON DELETE CASCADE,
    account_id UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
    transaction_type VARCHAR(50) NOT NULL, -- Buy, Sell, Deposit, Withdrawal, YieldAccrual, Dividend, Coupon, Fee, Tax, Split
    transaction_date TIMESTAMP WITH TIME ZONE NOT NULL,
    quantity NUMERIC(18, 8) NOT NULL DEFAULT 0,
    price_per_unit NUMERIC(18, 8) NOT NULL DEFAULT 0,
    total_amount NUMERIC(18, 4) NOT NULL DEFAULT 0,
    fee_amount NUMERIC(18, 4) NOT NULL DEFAULT 0,
    tax_amount NUMERIC(18, 4) NOT NULL DEFAULT 0,
    currency VARCHAR(10) NOT NULL DEFAULT 'BRL',
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_transactions_investment ON transactions(investment_id);
CREATE INDEX idx_transactions_account ON transactions(account_id);
CREATE INDEX idx_transactions_date ON transactions(transaction_date);

-- 5. Market Prices (Daily Closing Prices for Tickers)
CREATE TABLE market_prices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ticker VARCHAR(50) NOT NULL,
    price_date DATE NOT NULL,
    closing_price NUMERIC(18, 8) NOT NULL,
    currency VARCHAR(10) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_market_prices_ticker_date UNIQUE (ticker, price_date)
);

CREATE INDEX idx_market_prices_ticker_date ON market_prices(ticker, price_date);

-- 6. Economic Indexes (Daily Rates for CDI, TR, IPCA, etc.)
CREATE TABLE economic_indexes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    index_code VARCHAR(50) NOT NULL, -- CDI, TR, IPCA, SELIC
    index_date DATE NOT NULL,
    daily_rate NUMERIC(18, 8) NOT NULL, -- e.g. 0.000412 for CDI daily factor
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_economic_indexes_code_date UNIQUE (index_code, index_date)
);

CREATE INDEX idx_economic_indexes_code_date ON economic_indexes(index_code, index_date);

-- 7. Exchange Rates (FX Conversion Rates)
CREATE TABLE exchange_rates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    from_currency VARCHAR(10) NOT NULL,
    to_currency VARCHAR(10) NOT NULL,
    rate_date DATE NOT NULL,
    rate NUMERIC(18, 8) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_exchange_rates_pair_date UNIQUE (from_currency, to_currency, rate_date)
);

CREATE INDEX idx_exchange_rates_pair_date ON exchange_rates(from_currency, to_currency, rate_date);

-- 8. Portfolio Daily Snapshots (For historical chart rendering)
CREATE TABLE portfolio_daily_snapshots (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    account_id UUID REFERENCES accounts(id) ON DELETE CASCADE, -- NULL represents overall portfolio aggregate for user
    snapshot_date DATE NOT NULL,
    total_value_brl NUMERIC(18, 4) NOT NULL,
    total_value_usd NUMERIC(18, 4) NOT NULL,
    total_invested_brl NUMERIC(18, 4) NOT NULL,
    total_invested_usd NUMERIC(18, 4) NOT NULL,
    net_gain_loss_brl NUMERIC(18, 4) NOT NULL,
    net_gain_loss_usd NUMERIC(18, 4) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_portfolio_snapshots_user_account_date UNIQUE (user_id, account_id, snapshot_date)
);

CREATE INDEX idx_portfolio_snapshots_user_date ON portfolio_daily_snapshots(user_id, snapshot_date);

-- 9. PTAX Rates Table (Official BCB Buy & Sell Rates for Brazilian Tax / GCAP)
CREATE TABLE ptax_rates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    rate_date DATE NOT NULL UNIQUE,
    buy_rate NUMERIC(18, 8) NOT NULL,  -- Cotacao Compra
    sell_rate NUMERIC(18, 8) NOT NULL, -- Cotacao Venda
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_ptax_rates_date ON ptax_rates(rate_date);


