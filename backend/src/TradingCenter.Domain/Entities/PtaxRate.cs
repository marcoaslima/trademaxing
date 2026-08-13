namespace TradingCenter.Domain.Entities;

public class PtaxRate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime RateDate { get; set; }
    public decimal BuyRate { get; set; }  // Cotação Compra (used for sales / dividends)
    public decimal SellRate { get; set; } // Cotação Venda (used for acquisitions)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
