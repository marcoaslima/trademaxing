using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvestmentId { get; set; }
    public Investment Investment { get; set; } = null!;

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public TransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
