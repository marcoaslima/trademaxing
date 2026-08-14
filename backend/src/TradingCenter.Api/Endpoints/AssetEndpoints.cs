using FluentValidation;
using TradingCenter.Services.DTOs.Investment;
using TradingCenter.Services.Services;

namespace TradingCenter.Api.Endpoints;

public static class AssetEndpoints
{
    public static void MapAssetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/assets")
                       .WithTags("Master Asset Catalog");

        // Public / Authenticated search list of Master Assets
        group.MapGet("/", async (string? search, IInvestmentService investmentService, CancellationToken ct) =>
        {
            var assets = await investmentService.GetInvestmentsAsync(null, ct);
            // Search filter if provided
            return Results.Ok(assets);
        });

        // Create Master Asset (Maintainer / Admin)
        group.MapPost("/", async (CreateAssetDto dto, IInvestmentService investmentService, CancellationToken ct) =>
        {
            try
            {
                var created = await investmentService.CreateAssetAsync(dto, ct);
                return Results.Created($"/api/v1/assets", created);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }).RequireAuthorization();

        // Update Master Asset
        group.MapPut("/{id:guid}", async (Guid id, CreateAssetDto dto, IInvestmentService investmentService, CancellationToken ct) =>
        {
            var updated = await investmentService.UpdateAssetAsync(id, dto, ct);
            return updated != null ? Results.Ok(updated) : Results.NotFound();
        }).RequireAuthorization();

        // Delete Master Asset
        group.MapDelete("/{id:guid}", async (Guid id, IInvestmentService investmentService, CancellationToken ct) =>
        {
            var deleted = await investmentService.DeleteAssetAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization();
    }
}
