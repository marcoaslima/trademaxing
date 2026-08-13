using TradingCenter.Domain.Enums;

namespace TradingCenter.Services.DTOs.Investment;

public record InvestmentDto(
    Guid Id,
    Guid AccountId,
    Guid AssetId,
    string Name,
    string? Ticker,
    string? CustomName,
    AssetCategory AssetCategory,
    ValuationType ValuationType,
    Currency Currency,
    IndexBenchmark IndexBenchmark,
    decimal? InterestRate,
    DateTime? MaturityDate,
    string? LogoUrl,
    DateTime CreatedAt
);

public record CreateInvestmentDto(
    Guid AccountId,
    Guid AssetId,
    string? CustomName,
    decimal? InterestRate,
    DateTime? MaturityDate
);

public record CreateAssetDto(
    string Name,
    string? Ticker,
    AssetCategory AssetCategory,
    ValuationType ValuationType,
    Currency Currency,
    IndexBenchmark IndexBenchmark,
    string? LogoUrl
);
