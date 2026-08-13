using TradingCenter.Domain.Enums;

namespace TradingCenter.Services.DTOs.Investment;

public record InvestmentDto(
    Guid Id,
    Guid AccountId,
    string Name,
    string? Ticker,
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
    string Name,
    string? Ticker,
    AssetCategory AssetCategory,
    ValuationType ValuationType,
    Currency Currency,
    IndexBenchmark IndexBenchmark,
    decimal? InterestRate,
    DateTime? MaturityDate,
    string? LogoUrl
);
