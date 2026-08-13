using TradingCenter.Services.DTOs.Account;
using TradingCenter.Services.Services;

namespace TradingCenter.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/accounts")
                       .WithTags("Accounts")
                       .RequireAuthorization();

        group.MapGet("/", async (IAccountService accountService, CancellationToken ct) =>
        {
            var accounts = await accountService.GetUserAccountsAsync(ct);
            return Results.Ok(accounts);
        });

        group.MapGet("/{id:guid}", async (Guid id, IAccountService accountService, CancellationToken ct) =>
        {
            var account = await accountService.GetByIdAsync(id, ct);
            return account != null ? Results.Ok(account) : Results.NotFound();
        });

        group.MapPost("/", async (CreateAccountDto dto, IAccountService accountService, CancellationToken ct) =>
        {
            var created = await accountService.CreateAccountAsync(dto, ct);
            return Results.Created($"/api/v1/accounts/{created.Id}", created);
        });
    }
}
