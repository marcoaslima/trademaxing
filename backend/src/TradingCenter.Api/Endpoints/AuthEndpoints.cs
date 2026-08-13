using FluentValidation;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Auth;
using TradingCenter.Services.Services;

namespace TradingCenter.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Authentication");

        group.MapPost("/register", async (RegisterRequestDto dto, IAuthService authService, IValidator<RegisterRequestDto> validator, CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(dto, ct);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            try
            {
                var response = await authService.RegisterAsync(dto, ct);
                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        group.MapPost("/login", async (LoginRequestDto dto, IAuthService authService, CancellationToken ct) =>
        {
            try
            {
                var response = await authService.LoginAsync(dto, ct);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });

        group.MapGet("/me", (ICurrentUserService currentUserService) =>
        {
            return currentUserService.IsAuthenticated
                ? Results.Ok(new { userId = currentUserService.UserId })
                : Results.Unauthorized();
        }).RequireAuthorization();
    }
}
