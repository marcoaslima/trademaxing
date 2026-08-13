using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Enums;

namespace TradingCenter.Services.Services;

public interface IValuationCalculator
{
    decimal CalculateCurrentUnitPrice(Investment investment, decimal latestMarketPrice, decimal dailyIndexRate);
}

public class ValuationCalculator : IValuationCalculator
{
    public decimal CalculateCurrentUnitPrice(Investment investment, decimal latestMarketPrice, decimal dailyIndexRate)
    {
        return investment.ValuationType switch
        {
            ValuationType.TickerMarket => latestMarketPrice > 0 ? latestMarketPrice : 1.0m,
            ValuationType.IndexLinked => CalculateIndexAccrual(investment, dailyIndexRate),
            ValuationType.FixedRate => CalculateFixedRateYield(investment),
            ValuationType.ManualBalance => 1.0m,
            _ => 1.0m
        };
    }

    private static decimal CalculateIndexAccrual(Investment investment, decimal dailyIndexRate)
    {
        // For IndexLinked investments (e.g. FGTS TR+3% p.a. or CDB 100% CDI):
        // Calculate daily factor accrual multiplier
        var multiplier = investment.InterestRate ?? 1.0m; // e.g. 1.20 for 120% CDI
        var accruedFactor = 1.0m + (dailyIndexRate * multiplier);
        return accruedFactor;
    }

    private static decimal CalculateFixedRateYield(Investment investment)
    {
        // Fixed Rate yield (e.g. 5% p.a. for US Private Bond at par value 1.0)
        var annualRate = investment.InterestRate ?? 0.05m;
        return 1.0m + (annualRate / 365m);
    }
}
