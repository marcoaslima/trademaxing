using FluentValidation;
using TradingCenter.Services.DTOs.Investment;
using TradingCenter.Services.Services;

namespace TradingCenter.Api.Endpoints;

public static class InvestmentEndpoints
{
    public static void MapInvestmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/investments")
                       .WithTags("Investments")
                       .RequireAuthorization();

        group.MapGet("/", async (Guid? accountId, IInvestmentService investmentService, CancellationToken ct) =>
        {
            var list = await investmentService.GetInvestmentsAsync(accountId, ct);
            return Results.Ok(list);
        });

        group.MapGet("/{id:guid}", async (Guid id, IInvestmentService investmentService, CancellationToken ct) =>
        {
            var item = await investmentService.GetByIdAsync(id, ct);
            return item != null ? Results.Ok(item) : Results.NotFound();
        });

        group.MapPost("/", async (CreateInvestmentDto dto, IInvestmentService investmentService, IValidator<CreateInvestmentDto> validator, CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(dto, ct);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            try
            {
                var created = await investmentService.CreateInvestmentAsync(dto, ct);
                return Results.Created($"/api/v1/investments/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
    }
}
