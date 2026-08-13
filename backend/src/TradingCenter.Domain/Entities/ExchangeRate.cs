using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class ExchangeRate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Currency FromCurrency { get; set; } = Currency.USD;
    public Currency ToCurrency { get; set; } = Currency.BRL;
    public DateTime RateDate { get; set; }
    public decimal Rate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
