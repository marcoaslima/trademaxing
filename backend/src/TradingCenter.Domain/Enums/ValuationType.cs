namespace TradingCenter.Domain.Enums;

public enum ValuationType
{
    TickerMarket = 1, // Priced via stock exchange API (Yahoo Finance, B3)
    IndexLinked = 2,  // Priced via daily/monthly index accretion (FGTS TR+3%, CDB CDI, Tesouro IPCA)
    FixedRate = 3,    // Contractual fixed percentage or par value schedule
    ManualBalance = 4 // Manual user appraisal or ledger entry
}
