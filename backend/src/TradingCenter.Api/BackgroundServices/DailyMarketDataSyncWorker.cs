using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradingCenter.Services.Services;

namespace TradingCenter.Api.BackgroundServices;

public class DailyMarketDataSyncWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyMarketDataSyncWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(4);

    public DailyMarketDataSyncWorker(IServiceScopeFactory scopeFactory, ILogger<DailyMarketDataSyncWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyMarketDataSyncWorker background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Triggering automatic market data sync (PTAX USD, Stock Prices, CDI, TR)...");
                using var scope = _scopeFactory.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<IMarketDataSyncService>();
                await syncService.SyncAllMarketDataAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during daily market data sync.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
