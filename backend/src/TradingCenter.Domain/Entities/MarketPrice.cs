using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class MarketPrice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Ticker { get; set; } = string.Empty;
    public DateTime PriceDate { get; set; }
    public decimal ClosingPrice { get; set; }
    public Currency Currency { get; set; } = Currency.USD;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
