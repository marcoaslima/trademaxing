using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Auth;

namespace TradingCenter.Services.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
    {
        var existing = await _unitOfWork.Repository<User>().FindAsync(u => u.Email == dto.Email.ToLowerInvariant(), ct);
        if (existing.Any())
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new User
        {
            Email = dto.Email.ToLowerInvariant(),
            Name = dto.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _unitOfWork.Repository<User>().AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var token = GenerateJwtToken(user);
        return new AuthResponseDto(token, user.Id, user.Email, user.Name);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var users = await _unitOfWork.Repository<User>().FindAsync(u => u.Email == dto.Email.ToLowerInvariant(), ct);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = GenerateJwtToken(user);
        return new AuthResponseDto(token, user.Id, user.Email, user.Name);
    }

    private string GenerateJwtToken(User user)
    {
        var secret = _configuration["Jwt:Secret"] ?? "TradingCenter_Super_Secret_JWT_Key_2026_Minimum_32_Chars!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "TradingCenter",
            audience: _configuration["Jwt:Audience"] ?? "TradingCenterUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
