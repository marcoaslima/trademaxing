-- Seed Data for TradingCenter Master Assets Catalog

INSERT INTO assets (id, name, ticker, asset_category, valuation_type, currency, index_benchmark, logo_url, created_at)
VALUES
-- US Tickers
(gen_random_uuid(), 'Alphabet Inc. Class A', 'GOOGL', 'Stock_US', 'TickerMarket', 'USD', 'None', '/assets/logos/GOOG_mark_light.svg', NOW()),
(gen_random_uuid(), 'Apple Inc.', 'AAPL', 'Stock_US', 'TickerMarket', 'USD', 'None', '/assets/logos/AAPL_mark_light.svg', NOW()),
(gen_random_uuid(), 'Microsoft Corporation', 'MSFT', 'Stock_US', 'TickerMarket', 'USD', 'None', '/assets/logos/MSFT_mark_light.svg', NOW()),
(gen_random_uuid(), 'Advanced Micro Devices Inc.', 'AMD', 'Stock_US', 'TickerMarket', 'USD', 'None', '/assets/logos/AMD_mark_light.svg', NOW()),
(gen_random_uuid(), 'JPMorgan Chase & Co.', 'JPM', 'Stock_US', 'TickerMarket', 'USD', 'None', '/assets/logos/JPM_mark_light.svg', NOW()),

-- B3 Brazilian Tickers
(gen_random_uuid(), 'Petróleo Brasileiro S.A. - Petrobras', 'PETR4.SA', 'Stock_BR', 'TickerMarket', 'BRL', 'IBOVESPA', NULL, NOW()),
(gen_random_uuid(), 'Vale S.A.', 'VALE3.SA', 'Stock_BR', 'TickerMarket', 'BRL', 'IBOVESPA', NULL, NOW()),
(gen_random_uuid(), 'CSHG Logística FII', 'HGLG11.SA', 'REIT_BR', 'TickerMarket', 'BRL', 'None', NULL, NOW()),
(gen_random_uuid(), 'iShares S&P 500 Fundo de Índice', 'IVVB11.SA', 'Stock_BR', 'TickerMarket', 'BRL', 'SP500', NULL, NOW()),

-- Brazilian Government & Fixed Income (Non-Ticker / Index Linked)
(gen_random_uuid(), 'Tesouro SELIC 2035', NULL, 'Bond_BR_FixedIncome', 'IndexLinked', 'BRL', 'SELIC', NULL, NOW()),
(gen_random_uuid(), 'Tesouro IPCA+ 2035', NULL, 'Bond_BR_FixedIncome', 'IndexLinked', 'BRL', 'IPCA', NULL, NOW()),
(gen_random_uuid(), 'FGTS Caixa Econômica Federal', NULL, 'FGTS', 'IndexLinked', 'BRL', 'TR', NULL, NOW())
ON CONFLICT (ticker) DO NOTHING;
