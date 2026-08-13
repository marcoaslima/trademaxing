using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Currency BaseCurrency { get; set; } = Currency.BRL;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<PortfolioSnapshot> Snapshots { get; set; } = new List<PortfolioSnapshot>();
}
