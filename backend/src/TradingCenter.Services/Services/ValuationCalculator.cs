using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Enums;

namespace TradingCenter.Services.Services;

public interface IValuationCalculator
{
    decimal CalculateCurrentUnitPrice(Asset asset, decimal? customInterestRate, decimal latestMarketPrice, decimal dailyIndexRate);
}

public class ValuationCalculator : IValuationCalculator
{
    public decimal CalculateCurrentUnitPrice(Asset asset, decimal? customInterestRate, decimal latestMarketPrice, decimal dailyIndexRate)
    {
        return asset.ValuationType switch
        {
            ValuationType.TickerMarket => latestMarketPrice > 0 ? latestMarketPrice : 1.0m,
            ValuationType.IndexLinked => CalculateIndexAccrual(customInterestRate, dailyIndexRate),
            ValuationType.FixedRate => CalculateFixedRateYield(customInterestRate),
            ValuationType.ManualBalance => 1.0m,
            ValuationType.ManualFixedValue => 1.0m,
            _ => 1.0m
        };
    }

    private static decimal CalculateIndexAccrual(decimal? customInterestRate, decimal dailyIndexRate)
    {
        var multiplier = customInterestRate ?? 1.0m; // e.g. 1.20 for 120% CDI
        var accruedFactor = 1.0m + (dailyIndexRate * multiplier);
        return accruedFactor;
    }

    private static decimal CalculateFixedRateYield(decimal? customInterestRate)
    {
        var annualRate = customInterestRate ?? 0.05m;
        return 1.0m + (annualRate / 365m);
    }
}
