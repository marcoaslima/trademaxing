using AutoMapper;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Enums;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Portfolio;

namespace TradingCenter.Services.Services;

public interface IPortfolioEngine
{
    Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PortfolioSnapshotDto>> GetPortfolioHistoryAsync(CancellationToken ct = default);
}

public class PortfolioEngine : IPortfolioEngine
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValuationCalculator _valuationCalculator;
    private readonly IMapper _mapper;

    public PortfolioEngine(
        IUnitOfWork unitOfWork, 
        IValuationCalculator valuationCalculator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _valuationCalculator = valuationCalculator;
        _mapper = mapper;
    }

    public async Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(CancellationToken ct = default)
    {
        var investments = await _unitOfWork.Repository<Investment>().GetAllAsync(ct);
        var assets = await _unitOfWork.Repository<Asset>().GetAllAsync(ct);
        var transactions = await _unitOfWork.Repository<Transaction>().GetAllAsync(ct);
        var assetDict = assets.ToDictionary(a => a.Id);
        
        // Latest FX rate (USD -> BRL)
        var fxRates = await _unitOfWork.Repository<ExchangeRate>().GetAllAsync(ct);
        var latestFxRate = fxRates.OrderByDescending(r => r.RateDate).FirstOrDefault()?.Rate ?? 5.50m;

        // Latest market prices for tickers
        var marketPrices = await _unitOfWork.Repository<MarketPrice>().GetAllAsync(ct);
        var latestPriceDict = marketPrices
            .Where(m => !string.IsNullOrEmpty(m.Ticker))
            .GroupBy(m => m.Ticker, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(m => m.PriceDate).First().ClosingPrice, StringComparer.OrdinalIgnoreCase);

        // Latest economic indexes
        var economicIndexes = await _unitOfWork.Repository<EconomicIndex>().GetAllAsync(ct);
        var latestCdiRate = economicIndexes.Where(e => e.IndexCode == IndexBenchmark.CDI).OrderByDescending(e => e.IndexDate).FirstOrDefault()?.DailyRate ?? 0.0004m;

        var positions = new List<PositionSummaryDto>();

        decimal totalNetWorthBrl = 0;
        decimal totalNetWorthUsd = 0;
        decimal totalInvestedBrl = 0;
        decimal totalInvestedUsd = 0;

        foreach (var inv in investments)
        {
            if (!assetDict.TryGetValue(inv.AssetId, out var asset)) continue;

            var invTransactions = transactions.Where(t => t.InvestmentId == inv.Id).ToList();
            if (!invTransactions.Any()) continue;

            decimal totalQty = 0;
            decimal totalCost = 0;

            foreach (var tx in invTransactions)
            {
                switch (tx.TransactionType)
                {
                    case TransactionType.Buy:
                    case TransactionType.Deposit:
                    case TransactionType.YieldAccrual:
                        totalQty += tx.Quantity;
                        totalCost += tx.TotalAmount;
                        break;
                    case TransactionType.Sell:
                    case TransactionType.Withdrawal:
                        totalQty -= tx.Quantity;
                        totalCost -= tx.TotalAmount;
                        break;
                }
            }

            if (totalQty <= 0) continue;

            decimal avgPrice = totalQty > 0 ? totalCost / totalQty : 0;
            
            decimal marketPrice = avgPrice;
            if (!string.IsNullOrEmpty(asset.Ticker) && latestPriceDict.TryGetValue(asset.Ticker, out var syncedPrice) && syncedPrice > 0)
            {
                marketPrice = syncedPrice;
            }

            decimal unitPrice = _valuationCalculator.CalculateCurrentUnitPrice(asset, inv.InterestRate, marketPrice, latestCdiRate);
            decimal currentValue = totalQty * unitPrice;
            decimal pnl = currentValue - totalCost;
            decimal pnlPct = totalCost > 0 ? (pnl / totalCost) * 100m : 0;

            var displayName = !string.IsNullOrEmpty(inv.CustomName) ? inv.CustomName : asset.Name;

            if (asset.Currency == Currency.BRL)
            {
                totalNetWorthBrl += currentValue;
                totalInvestedBrl += totalCost;
                totalNetWorthUsd += currentValue / latestFxRate;
                totalInvestedUsd += totalCost / latestFxRate;
            }
            else
            {
                totalNetWorthUsd += currentValue;
                totalInvestedUsd += totalCost;
                totalNetWorthBrl += currentValue * latestFxRate;
                totalInvestedBrl += totalCost * latestFxRate;
            }

            positions.Add(new PositionSummaryDto(
                inv.Id,
                displayName,
                asset.Ticker,
                asset.AssetCategory.ToString(),
                asset.ValuationType.ToString(),
                totalQty,
                avgPrice,
                totalCost,
                unitPrice,
                currentValue,
                pnl,
                pnlPct,
                asset.Currency.ToString(),
                asset.LogoUrl
            ));
        }

        return new PortfolioSummaryDto(
            totalNetWorthBrl,
            totalNetWorthUsd,
            totalInvestedBrl,
            totalInvestedUsd,
            totalNetWorthBrl - totalInvestedBrl,
            totalNetWorthUsd - totalInvestedUsd,
            latestFxRate,
            positions
        );
    }

    public async Task<IReadOnlyList<PortfolioSnapshotDto>> GetPortfolioHistoryAsync(CancellationToken ct = default)
    {
        var snapshots = await _unitOfWork.Repository<PortfolioSnapshot>().GetAllAsync(ct);
        var sorted = snapshots.OrderBy(s => s.SnapshotDate).ToList();
        return _mapper.Map<IReadOnlyList<PortfolioSnapshotDto>>(sorted);
    }
}
