using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
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

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true));
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true));
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization();

// 4. OpenAPI / Swagger Definition
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. Background Hosted Services
builder.Services.AddHostedService<DailyMarketDataSyncWorker>();

var app = builder.Build();

// Ensure database schema contains password_hash column on existing Postgres volumes
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TradingCenter.Repository.Context.AppDbContext>();
    dbContext.Database.ExecuteSqlRaw("ALTER TABLE users ADD COLUMN IF NOT EXISTS password_hash VARCHAR(255);");
    dbContext.Database.ExecuteSqlRaw("UPDATE assets SET ticker = SUBSTRING(ticker FROM POSITION(':' IN ticker) + 1) WHERE ticker LIKE '%:%';");
}
catch (Exception ex)
{
    Console.WriteLine($"Database schema check warning: {ex.Message}");
}

// Configure Middleware Pipeline
app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Map Minimal API Endpoint Groups
app.MapAuthEndpoints();
app.MapAssetEndpoints();
app.MapAccountEndpoints();
app.MapInvestmentEndpoints();
app.MapTransactionEndpoints();
app.MapPortfolioEndpoints();
app.MapMarketDataEndpoints();

app.Run();
