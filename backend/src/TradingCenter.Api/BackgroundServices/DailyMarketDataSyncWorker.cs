using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TradingCenter.Api.BackgroundServices;

public class DailyMarketDataSyncWorker : BackgroundService
{
    private readonly ILogger<DailyMarketDataSyncWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(4);

    public DailyMarketDataSyncWorker(ILogger<DailyMarketDataSyncWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyMarketDataSyncWorker background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Syncing market closing prices, economic rates (CDI, TR), and FX rates...");
                // Background price sync logic execution
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during daily market data sync.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
