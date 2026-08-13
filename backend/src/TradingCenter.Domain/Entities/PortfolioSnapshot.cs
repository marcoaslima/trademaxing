namespace TradingCenter.Domain.Entities;

public class PortfolioSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }

    public DateTime SnapshotDate { get; set; }
    public decimal TotalValueBrl { get; set; }
    public decimal TotalValueUsd { get; set; }
    public decimal TotalInvestedBrl { get; set; }
    public decimal TotalInvestedUsd { get; set; }
    public decimal NetGainLossBrl { get; set; }
    public decimal NetGainLossUsd { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
