using FluentValidation;
using TradingCenter.Services.DTOs.Transaction;
using TradingCenter.Services.Services;

namespace TradingCenter.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/transactions")
                       .WithTags("Transactions")
                       .RequireAuthorization();

        group.MapGet("/", async (Guid? investmentId, ITransactionService transactionService, CancellationToken ct) =>
        {
            var list = await transactionService.GetTransactionsAsync(investmentId, ct);
            return Results.Ok(list);
        });

        group.MapPost("/", async (CreateTransactionDto dto, ITransactionService transactionService, IValidator<CreateTransactionDto> validator, CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(dto, ct);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            try
            {
                var created = await transactionService.CreateTransactionAsync(dto, ct);
                return Results.Created($"/api/v1/transactions/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
    }
}
