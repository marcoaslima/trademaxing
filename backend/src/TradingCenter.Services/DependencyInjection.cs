using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TradingCenter.Services.Mappings;
using TradingCenter.Services.Services;
using TradingCenter.Services.Validators;

namespace TradingCenter.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IInvestmentService, InvestmentService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IValuationCalculator, ValuationCalculator>();
        services.AddScoped<IPortfolioEngine, PortfolioEngine>();
        services.AddHttpClient<IMarketDataSyncService, MarketDataSyncService>();

        return services;
    }
}
