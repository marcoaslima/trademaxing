
application to track my investments accross multiple types and brokers

general features:
    - users (support multiple users)
    - accounts (support multiple accounts per user, e.g. personal, joint, etc.)
    - investments (support multiple investments per account)
    - transactions (support multiple transactions per investment)


technical requirements:
    - Language: C# .NET
    - Database: PostgreSQL
    - ORM: Entity Framework
    - API: REST
    - prefer to use minimal abstractions
    - prefer to use minimal external libraries
    - prefer to use minimal external services
    - prefer to use minimal external dependencies



UI requirements:
    - Clean and modern interface
    - Support for multiple users
    - Support for multiple accounts
    - Support for multiple investments
    - Support for multiple transactions
    

MVP - Only the backend. No UI, just the API.

Every day at the closing or of the market a script will have to run to update the values of the investments (for stocks, funds, etfs, etc) against the necessary apis to get dollar prices, stock prices and such. 

Every time the user adds an inestiment the system needs to fetch the current price of the investment to set the initial value of the investment.

Future features:
    - add CSV import for transactions
    - add support for multiple currencies
    - add support for multiple brokers
    - add support for multiple investment types
    - add support for multiple transaction types
    - add support for multiple fund indexes
    - add support for multiple fund subtypes
    - add support for multiple bond types
    - add support for multiple p2p lending types
    - add support for multiple reit types
    - add support for multiple stock types
    - add support for multiple bond types