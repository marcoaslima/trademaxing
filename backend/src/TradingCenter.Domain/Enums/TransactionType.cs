namespace TradingCenter.Domain.Enums;

public enum TransactionType
{
    Buy = 1,
    Sell = 2,
    Deposit = 3,       // Employer or voluntary deposit (e.g. FGTS)
    Withdrawal = 4,    // Saque-aniversário, Saque rescisão, Redemption
    YieldAccrual = 5,  // Rendimento (Interest accretion)
    Dividend = 6,
    Coupon = 7,        // Bond coupon interest payment
    Fee = 8,
    Tax = 9,           // IR / Withholding tax
    Split = 10,
    Bonus = 11
}
