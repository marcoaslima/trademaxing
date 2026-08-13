using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Repository.Context;
using TradingCenter.Repository.Repositories;

namespace TradingCenter.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=tradingcenter;Username=postgres;Password=postgres_dev_pass";

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
