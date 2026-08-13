namespace TradingCenter.Services.DTOs.Auth;

public record RegisterRequestDto(string Email, string Name, string Password);
public record LoginRequestDto(string Email, string Password);
public record AuthResponseDto(string Token, Guid UserId, string Email, string Name);
