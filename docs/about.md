# TradingCenter Investment Tracking System

Application to track personal investments across multiple asset classes (Stocks, ETFs, US Private Bonds, FGTS, BRL Fixed Income, REITs) and institutions/brokers in BRL and USD.

## Core Domain Hierarchy
- **Users**: Multi-user support with custom base currency (BRL / USD).
- **Accounts**: Custody/brokerage accounts per user (e.g. Caixa FGTS, Avenue, Interactive Brokers, XP).
- **Investments (Assets)**: Assets held in accounts (ticker-based or contractual non-ticker assets like FGTS and Private Bonds).
- **Transactions**: Financial movement ledger (Buy, Sell, Deposit, Withdrawal, Yield Accrual, Dividend, Coupon, Tax, Fee).

## Technical Requirements
- **Language**: C# .NET (Minimal APIs)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **API**: REST
- **Design Philosophy**: Minimal abstractions, minimal external dependencies, clean data structures.

## Data Caching & External Financial API Strategy
- **Store-and-Serve Architecture**: User HTTP requests **NEVER** trigger live calls to external financial APIs. Instead, the backend reads prices from local database tables (`market_prices`, `economic_indexes`, `exchange_rates`) and .NET `IMemoryCache`.
- **Scheduled Background Polling**: A C# `IHostedService` runs periodically (e.g. 4 times a day during market hours: 10:00, 13:00, 16:00, 18:00 UTC) to fetch updated prices for unique active tickers and economic rates.
- **Recommended Free APIs**:
  - **Yahoo Finance** (`USDBRL=X`, US Stocks, B3 Stocks e.g. `PETR4.SA`, `HGLG11.SA`): Free global prices.
  - **Banco Central do Brasil (BCB) API**: Official free REST API for daily CDI, Selic, TR, IPCA, and USD/BRL official rates.
  - **Brapi (brapi.dev)**: Free tier for B3 stocks, FIIs, and Brazilian inflation/interest indicators.

## MVP Scope (Backend API)
- REST API endpoints for User, Account, Asset, Transaction CRUD and Portfolio Net Worth consolidation.
- Manual transaction & asset entry by the user.
- Scheduled background fetcher for stock tickers, economic indexes, and exchange rates.
- In-memory pricing cache layer (`IMemoryCache`).

## Deployment & Infrastructure Strategy
- **Deployment Platform**: Dokploy (Self-hosted PaaS on dev server)
- **Containerization**: Docker & Docker Compose (`docker-compose.yml`)
- **Container Architecture**:
  - `backend-api`: C# .NET Minimal API container (multi-stage `Dockerfile`)
  - `postgres-db`: PostgreSQL 16 container with persistent volumes
  - Automatic environment configuration & database migration execution on startup

## Future Roadmap
- B3 Open Finance API integration & automated account sync.
- CSV transaction import.
- Advanced tax reporting (IR regressivo / GCAP).
- Frontend UI interface.


