using TradingCenter.Services.Services;

namespace TradingCenter.Api.Endpoints;

public static class MarketDataEndpoints
{
    public static void MapMarketDataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/market-data")
                       .WithTags("Market Data Synchronization")
                       .RequireAuthorization();

        group.MapPost("/sync", async (IMarketDataSyncService syncService, CancellationToken ct) =>
        {
            try
            {
                await syncService.SyncAllMarketDataAsync(ct);
                return Results.Ok(new { message = "Market data synchronization triggered successfully." });
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 500, title: "Market data sync error");
            }
        });
    }
}
