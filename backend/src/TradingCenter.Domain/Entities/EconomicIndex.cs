using TradingCenter.Domain.Enums;

namespace TradingCenter.Domain.Entities;

public class EconomicIndex
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public IndexBenchmark IndexCode { get; set; }
    public DateTime IndexDate { get; set; }
    public decimal DailyRate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
