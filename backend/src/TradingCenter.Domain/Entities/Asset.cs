using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Ticker { get; set; }
    public AssetCategory AssetCategory { get; set; }
    public ValuationType ValuationType { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
    public IndexBenchmark IndexBenchmark { get; set; } = IndexBenchmark.None;
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Investment> Investments { get; set; } = new List<Investment>();
}
