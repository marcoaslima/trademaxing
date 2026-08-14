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
)
{
    public PositionSummaryDto() : this(Guid.Empty, string.Empty, null, string.Empty, string.Empty, 0, 0, 0, 0, 0, 0, 0, string.Empty, null) { }
}

public record PortfolioSummaryDto(
    decimal TotalNetWorthBrl,
    decimal TotalNetWorthUsd,
    decimal TotalInvestedBrl,
    decimal TotalInvestedUsd,
    decimal TotalGainLossBrl,
    decimal TotalGainLossUsd,
    decimal ExchangeRateUsdBrl,
    IReadOnlyList<PositionSummaryDto> Positions
)
{
    public PortfolioSummaryDto() : this(0, 0, 0, 0, 0, 0, 0, new List<PositionSummaryDto>()) { }
}

public record PortfolioSnapshotDto(
    DateTime Date,
    decimal TotalValueBrl,
    decimal TotalValueUsd,
    decimal TotalInvestedBrl,
    decimal TotalInvestedUsd,
    decimal NetGainLossBrl,
    decimal NetGainLossUsd
)
{
    public PortfolioSnapshotDto() : this(default, 0, 0, 0, 0, 0, 0) { }
}
