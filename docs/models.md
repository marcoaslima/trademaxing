# Domain Models Specification

## 1. Users
- **Id**: Guid (Primary Key)
- **Email**: string
- **Name**: string
- **BaseCurrency**: Currency (BRL | USD)
- **CreatedAt**: DateTime

## 2. Accounts (Custody / Brokers / Financial Institutions)
- **Id**: Guid (Primary Key)
- **UserId**: Guid (Foreign Key -> Users)
- **Name**: string (e.g. "Caixa FGTS Account", "Avenue US Brokerage", "XP Renda Fixa")
- **Institution**: string (e.g. "Caixa Econômica Federal", "Avenue", "XP Investimentos", "Interactive Brokers")
- **AccountType**: Enum (`Personal`, `Joint`, `Retirement_FGTS`, `Brokerage`)
- **BaseCurrency**: Currency (`BRL`, `USD`)

## 3. Investments (Assets)
- **Id**: Guid (Primary Key)
- **AccountId**: Guid (Foreign Key -> Accounts)
- **Name**: string (e.g. "FGTS Contrato X", "AAPL", "US Private Bond 6% 2029", "CDB Banco Master 120% CDI")
- **Ticker**: string (optional) (e.g. "AAPL", "IVVB11", "HGLG11")
- **AssetCategory**: Enum:
  - `Stock_US`
  - `Stock_BR`
  - `Bond_US_Public`
  - `Bond_US_Private`
  - `Bond_BR_FixedIncome` (CDB, LCI, LCA, CRI, CRA, Tesouro)
  - `FGTS`
  - `Fund_US`
  - `Fund_BR`
  - `REIT_US`
  - `REIT_BR` (FII)
  - `P2P_Lending`
- **ValuationType**: Enum:
  - `TickerMarket`: Priced via stock exchange ticker API (e.g. Yahoo Finance, B3)
  - `IndexLinked`: Priced via rate accretion (e.g. TR+3% for FGTS, 100% CDI for CDB, IPCA+ for Tesouro)
  - `FixedRate`: Contractual fixed percentage return or par value schedule
  - `ManualBalance`: User manual appraisal or direct ledger entry
- **Currency**: Enum (`BRL`, `USD`, `EUR`, etc.)
- **IndexBenchmark**: Enum (optional) (`CDI`, `IPCA`, `TR`, `SELIC`, `SP500`, `IBOVESPA`, `None`)
- **InterestRate**: decimal (optional) (e.g. 1.20 for 120% CDI; 0.05 for 5% p.a.; 0.03 for TR+3%)
- **MaturityDate**: DateTime (optional)
- **Logo**: URL to S3/GCP/MinIO/Other hosting service

## 4. Transactions
- **Id**: Guid (Primary Key)
- **AssetId**: Guid (Foreign Key -> Investments)
- **AccountId**: Guid (Foreign Key -> Accounts)
- **TransactionType**: Enum:
  - `Buy`
  - `Sell`
  - `Deposit` (Employer/Voluntary deposits like FGTS monthly credit)
  - `Withdrawal` (Saque-aniversário, Saque rescisão, Redemption)
  - `YieldAccrual` (Rendimento / Monthly or periodic interest)
  - `Dividend`
  - `Coupon` (Bond interest payment)
  - `Fee`
  - `Tax` (IR / Withholding tax)
  - `Split`
  - `Bonus`
- **Date**: DateTime
- **Quantity**: decimal
- **PricePerUnit**: decimal
- **TotalAmount**: decimal
- **FeeAmount**: decimal
- **TaxAmount**: decimal
- **Currency**: Enum (`BRL`, `USD`)
- **Notes**: string (optional)

## 5. Market Closing Prices (Daily Stock/ETF Prices)
- **Id**: Guid (Primary Key)
- **Ticker**: string
- **Date**: DateTime
- **ClosingPrice**: decimal
- **Currency**: Enum (`BRL`, `USD`)

## 6. Economic Indexes (Daily Accretion Rates)
- **Id**: Guid (Primary Key)
- **IndexCode**: Enum (`CDI`, `TR`, `IPCA`, `SELIC`)
- **Date**: DateTime
- **DailyRate**: decimal (e.g. 0.000412 for daily CDI rate)

## 7. Exchange Rates (FX Conversion)
- **Id**: Guid (Primary Key)
- **FromCurrency**: Enum (`USD`)
- **ToCurrency**: Enum (`BRL`)
- **Date**: DateTime
- **Rate**: decimal (e.g. 5.50 BRL per 1 USD)

## 8. Portfolio Daily Snapshots (Historical Net Worth & Performance Charting)
- **Id**: Guid (Primary Key)
- **UserId**: Guid (Foreign Key -> Users)
- **AccountId**: Guid (optional, Foreign Key -> Accounts; null means total user portfolio)
- **SnapshotDate**: Date
- **TotalValueBrl**: decimal
- **TotalValueUsd**: decimal
- **TotalInvestedBrl**: decimal
- **TotalInvestedUsd**: decimal
- **NetGainLossBrl**: decimal
- **NetGainLossUsd**: decimal

## 9. PTAX Rates (Official BCB USD/BRL Rates for Brazilian Tax & GCAP)
- **Id**: Guid (Primary Key)
- **RateDate**: Date (Unique)
- **BuyRate**: decimal (Cotação Compra - used for sales & dividends)
- **SellRate**: decimal (Cotação Venda - used for acquisitions)


