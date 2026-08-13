using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public AccountType AccountType { get; set; } = AccountType.Personal;
    public Currency BaseCurrency { get; set; } = Currency.BRL;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<PortfolioSnapshot> Snapshots { get; set; } = new List<PortfolioSnapshot>();
}
