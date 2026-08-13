using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class Investment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Ticker { get; set; }
    public AssetCategory AssetCategory { get; set; }
    public ValuationType ValuationType { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
    public IndexBenchmark IndexBenchmark { get; set; } = IndexBenchmark.None;
    public decimal? InterestRate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
