namespace TradingCenter.Services.DTOs.Portfolio;

public record PositionSummaryDto(
    Guid InvestmentId,
    string Name,
    string? Ticker,
    string AssetCategory,
    string ValuationType,
    decimal Quantity,
    decimal AveragePrice,
    decimal TotalInvested,
    decimal CurrentUnitPrice,
    decimal CurrentTotalValue,
    decimal UnrealizedGainLoss,
    decimal UnrealizedGainLossPercentage,
    string Currency,
    string? LogoUrl
);

public record PortfolioSummaryDto(
    decimal TotalNetWorthBrl,
    decimal TotalNetWorthUsd,
    decimal TotalInvestedBrl,
    decimal TotalInvestedUsd,
    decimal TotalGainLossBrl,
    decimal TotalGainLossUsd,
    decimal ExchangeRateUsdBrl,
    IReadOnlyList<PositionSummaryDto> Positions
);

public record PortfolioSnapshotDto(
    DateTime Date,
    decimal TotalValueBrl,
    decimal TotalValueUsd,
    decimal TotalInvestedBrl,
    decimal TotalInvestedUsd,
    decimal NetGainLossBrl,
    decimal NetGainLossUsd
);
