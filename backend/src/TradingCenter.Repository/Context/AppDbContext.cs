using Microsoft.EntityFrameworkCore;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Interfaces;

namespace TradingCenter.Repository.Context;

public class AppDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Investment> Investments => Set<Investment>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<MarketPrice> MarketPrices => Set<MarketPrice>();
    public DbSet<EconomicIndex> EconomicIndexes => Set<EconomicIndex>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<PortfolioSnapshot> PortfolioSnapshots => Set<PortfolioSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Multitenancy Global Query Filters
        if (_currentUserService?.UserId.HasValue == true)
        {
            var userId = _currentUserService.UserId.Value;
            modelBuilder.Entity<Account>().HasQueryFilter(a => a.UserId == userId);
            modelBuilder.Entity<Investment>().HasQueryFilter(i => i.Account.UserId == userId);
            modelBuilder.Entity<Transaction>().HasQueryFilter(t => t.Account.UserId == userId);
            modelBuilder.Entity<PortfolioSnapshot>().HasQueryFilter(s => s.UserId == userId);
        }

        // Entity Configurations & Decimal Precision
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.BaseCurrency).HasConversion<string>().HasMaxLength(10);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Accounts)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Institution).HasMaxLength(255).IsRequired();
            entity.Property(e => e.AccountType).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.BaseCurrency).HasConversion<string>().HasMaxLength(10);
        });

        modelBuilder.Entity<Investment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Investments)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Ticker).HasMaxLength(50);
            entity.Property(e => e.AssetCategory).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.ValuationType).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Currency).HasConversion<string>().HasMaxLength(10);
            entity.Property(e => e.IndexBenchmark).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.InterestRate).HasPrecision(18, 6);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);

            entity.HasIndex(e => e.Ticker);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Investment)
                  .WithMany(i => i.Transactions)
                  .HasForeignKey(e => e.InvestmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.TransactionType).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Quantity).HasPrecision(18, 8);
            entity.Property(e => e.PricePerUnit).HasPrecision(18, 8);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
            entity.Property(e => e.FeeAmount).HasPrecision(18, 4);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 4);
            entity.Property(e => e.Currency).HasConversion<string>().HasMaxLength(10);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.TransactionDate);
        });

        modelBuilder.Entity<MarketPrice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Ticker, e.PriceDate }).IsUnique();
            entity.Property(e => e.Ticker).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ClosingPrice).HasPrecision(18, 8);
            entity.Property(e => e.Currency).HasConversion<string>().HasMaxLength(10);
        });

        modelBuilder.Entity<EconomicIndex>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.IndexCode, e.IndexDate }).IsUnique();
            entity.Property(e => e.IndexCode).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.DailyRate).HasPrecision(18, 8);
        });

        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FromCurrency, e.ToCurrency, e.RateDate }).IsUnique();
            entity.Property(e => e.FromCurrency).HasConversion<string>().HasMaxLength(10);
            entity.Property(e => e.ToCurrency).HasConversion<string>().HasMaxLength(10);
            entity.Property(e => e.Rate).HasPrecision(18, 8);
        });

        modelBuilder.Entity<PortfolioSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.AccountId, e.SnapshotDate }).IsUnique();
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Snapshots)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Snapshots)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.TotalValueBrl).HasPrecision(18, 4);
            entity.Property(e => e.TotalValueUsd).HasPrecision(18, 4);
            entity.Property(e => e.TotalInvestedBrl).HasPrecision(18, 4);
            entity.Property(e => e.TotalInvestedUsd).HasPrecision(18, 4);
            entity.Property(e => e.NetGainLossBrl).HasPrecision(18, 4);
            entity.Property(e => e.NetGainLossUsd).HasPrecision(18, 4);
        });
    }
}
