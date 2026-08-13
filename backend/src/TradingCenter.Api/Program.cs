using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TradingCenter.Api.BackgroundServices;
using TradingCenter.Api.Endpoints;
using TradingCenter.Api.Services;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Repository;
using TradingCenter.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Services & Composition Root
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// 2. Layer Services Registration
builder.Services.AddRepositoryServices(builder.Configuration);
builder.Services.AddApplicationServices();

// 3. JWT Authentication Configuration
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "TradingCenter_Super_Secret_JWT_Key_2026_Minimum_32_Chars!";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "TradingCenter",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "TradingCenterUsers",
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 4. OpenAPI / Swagger Definition
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. Background Hosted Services
builder.Services.AddHostedService<DailyMarketDataSyncWorker>();

var app = builder.Build();

// Configure Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Map Minimal API Endpoint Groups
app.MapAuthEndpoints();
app.MapAccountEndpoints();
app.MapInvestmentEndpoints();
app.MapTransactionEndpoints();
app.MapPortfolioEndpoints();

app.Run();
