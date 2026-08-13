
Models:
    Investiments:
        - Name
        - Ticker
        - Type (external key)
        - Subtype (external key)
        - Currency (external key)
        - Broker (external key)
    
    Investiment types:
        - Stocks US
        - Stocks BR
        - Bonds US
        - Bonds BR
        - Funds US
        - Funds BR
        - P2P Lending
        - REITS

    Transactions:
        - Date
        - Broker
        - Quantity
        - Price
        - Currency
        - Type
        

    Transactions types:
        - Buy
        - Sell
        - Dividend
        - Fee
        - Splitt
        - Bonus

    Fund Indexes:
        - S&P 500
        - NASDAQ
        - DOW JONES
        - IBOVESPA
        - CDI
        - IPCA

    Currencies:
        - USD
        - BRL
        - EUR
        - JPY
        - GBP
        - CHF
        - CAD
        - AUD
    
    Brokers:
        - Avenue
        - Interactive Brokers
        - Nomad
        - C6 Bank
        - Banco Inter
        - XP Investimentos
        - NuInvest
        