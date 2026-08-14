namespace TradingCenter.Domain.Enums;

public enum AssetCategory
{
    Stock_US = 1,
    Stock_BR = 2,
    Bond_US_Public = 3,
    Bond_US_Private = 4,
    Bond_BR_FixedIncome = 5, // CDB, LCI, LCA, CRI, CRA, Tesouro
    FGTS = 6,
    Fund_US = 7,
    Fund_BR = 8,
    REIT_US = 9,
    REIT_BR = 10, // FII
    P2P_Lending = 11,
    Etf_BR = 12,
    Etf_US = 13,
    FixedIncome_BR = 14,
    Crypto = 15,
    Cash = 16
}
