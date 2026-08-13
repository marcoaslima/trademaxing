namespace TradingCenter.Services.Services;

public interface IMarketDataSyncService
{
    Task SyncPtaxRatesAsync(DateTime? targetDate = null, CancellationToken ct = default);
    Task SyncStockPricesAsync(CancellationToken ct = default);
    Task SyncEconomicIndexesAsync(CancellationToken ct = default);
    Task SyncAllMarketDataAsync(CancellationToken ct = default);
}
