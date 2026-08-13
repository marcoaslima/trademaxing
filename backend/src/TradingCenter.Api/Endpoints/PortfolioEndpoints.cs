using TradingCenter.Services.Services;

namespace TradingCenter.Api.Endpoints;

public static class PortfolioEndpoints
{
    public static void MapPortfolioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/portfolio")
                       .WithTags("Portfolio Analytics")
                       .RequireAuthorization();

        group.MapGet("/summary", async (IPortfolioEngine portfolioEngine, CancellationToken ct) =>
        {
            var summary = await portfolioEngine.GetPortfolioSummaryAsync(ct);
            return Results.Ok(summary);
        });

        group.MapGet("/history", async (IPortfolioEngine portfolioEngine, CancellationToken ct) =>
        {
            var history = await portfolioEngine.GetPortfolioHistoryAsync(ct);
            return Results.Ok(history);
        });
    }
}
