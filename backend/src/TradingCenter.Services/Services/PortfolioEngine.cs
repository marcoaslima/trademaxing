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
        var transactions = await _unitOfWork.Repository<Transaction>().GetAllAsync(ct);
        
        // Latest FX rate (USD -> BRL)
        var fxRates = await _unitOfWork.Repository<ExchangeRate>().GetAllAsync(ct);
        var latestFxRate = fxRates.OrderByDescending(r => r.RateDate).FirstOrDefault()?.Rate ?? 5.50m;

        var positions = new List<PositionSummaryDto>();

        decimal totalNetWorthBrl = 0;
        decimal totalNetWorthUsd = 0;
        decimal totalInvestedBrl = 0;
        decimal totalInvestedUsd = 0;

        foreach (var inv in investments)
        {
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
            decimal unitPrice = _valuationCalculator.CalculateCurrentUnitPrice(inv, avgPrice, 0.0004m);
            decimal currentValue = totalQty * unitPrice;
            decimal pnl = currentValue - totalCost;
            decimal pnlPct = totalCost > 0 ? (pnl / totalCost) * 100m : 0;

            if (inv.Currency == Currency.BRL)
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
                inv.Name,
                inv.Ticker,
                inv.AssetCategory.ToString(),
                inv.ValuationType.ToString(),
                totalQty,
                avgPrice,
                totalCost,
                unitPrice,
                currentValue,
                pnl,
                pnlPct,
                inv.Currency.ToString(),
                inv.LogoUrl
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
