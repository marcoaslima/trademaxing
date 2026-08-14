using AutoMapper;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Investment;

namespace TradingCenter.Services.Services;

public interface IInvestmentService
{
    Task<IReadOnlyList<Asset>> GetAssetsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<InvestmentDto>> GetInvestmentsAsync(Guid? accountId = null, CancellationToken ct = default);
    Task<InvestmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentDto dto, CancellationToken ct = default);
    Task<InvestmentDto?> UpdateInvestmentAsync(Guid id, CreateInvestmentDto dto, CancellationToken ct = default);
    Task<bool> DeleteInvestmentAsync(Guid id, CancellationToken ct = default);
    Task<CreateAssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken ct = default);
    Task<Asset?> UpdateAssetAsync(Guid id, CreateAssetDto dto, CancellationToken ct = default);
    Task<bool> DeleteAssetAsync(Guid id, CancellationToken ct = default);
}

public class InvestmentService : IInvestmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InvestmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Asset>> GetAssetsAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Asset>().GetAllAsync(ct);
    }

    public async Task<IReadOnlyList<InvestmentDto>> GetInvestmentsAsync(Guid? accountId = null, CancellationToken ct = default)
    {
        var investments = accountId.HasValue 
            ? await _unitOfWork.Repository<Investment>().FindAsync(i => i.AccountId == accountId.Value, ct)
            : await _unitOfWork.Repository<Investment>().GetAllAsync(ct);

        var assets = await _unitOfWork.Repository<Asset>().GetAllAsync(ct);
        var assetDict = assets.ToDictionary(a => a.Id);

        foreach (var inv in investments)
        {
            if (assetDict.TryGetValue(inv.AssetId, out var asset))
            {
                inv.Asset = asset;
            }
        }

        return _mapper.Map<IReadOnlyList<InvestmentDto>>(investments);
    }

    public async Task<InvestmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _unitOfWork.Repository<Investment>().GetByIdAsync(id, ct);
        if (item == null) return null;

        var asset = await _unitOfWork.Repository<Asset>().GetByIdAsync(item.AssetId, ct);
        if (asset != null) item.Asset = asset;

        return _mapper.Map<InvestmentDto>(item);
    }

    public async Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentDto dto, CancellationToken ct = default)
    {
        var asset = await _unitOfWork.Repository<Asset>().GetByIdAsync(dto.AssetId, ct)
            ?? throw new ArgumentException($"Master Asset with ID '{dto.AssetId}' not found.");

        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(dto.AccountId, ct)
            ?? throw new ArgumentException($"Broker Account with ID '{dto.AccountId}' not found.");

        // Re-use existing investment if present for this account and asset
        var existingList = await _unitOfWork.Repository<Investment>().FindAsync(i => i.AccountId == dto.AccountId && i.AssetId == dto.AssetId, ct);
        var existing = existingList.FirstOrDefault();
        if (existing != null)
        {
            existing.Asset = asset;
            existing.Account = account;
            return _mapper.Map<InvestmentDto>(existing);
        }

        var investment = _mapper.Map<Investment>(dto);
        investment.Asset = asset;
        investment.Account = account;

        await _unitOfWork.Repository<Investment>().AddAsync(investment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<InvestmentDto>(investment);
    }

    public async Task<InvestmentDto?> UpdateInvestmentAsync(Guid id, CreateInvestmentDto dto, CancellationToken ct = default)
    {
        var repository = _unitOfWork.Repository<Investment>();
        var investment = await repository.GetByIdAsync(id, ct);
        if (investment == null) return null;

        var asset = await _unitOfWork.Repository<Asset>().GetByIdAsync(dto.AssetId, ct)
            ?? throw new ArgumentException($"Master Asset with ID '{dto.AssetId}' not found.");

        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(dto.AccountId, ct)
            ?? throw new ArgumentException($"Broker Account with ID '{dto.AccountId}' not found.");

        investment.AccountId = dto.AccountId;
        investment.AssetId = dto.AssetId;
        investment.CustomName = dto.CustomName;
        investment.InterestRate = dto.InterestRate;
        investment.MaturityDate = dto.MaturityDate;
        investment.Asset = asset;
        investment.Account = account;

        // Also update accountId on existing transactions linked to this investment
        var transactions = await _unitOfWork.Repository<Transaction>().FindAsync(t => t.InvestmentId == id, ct);
        foreach (var tx in transactions)
        {
            tx.AccountId = dto.AccountId;
            _unitOfWork.Repository<Transaction>().Update(tx);
        }

        repository.Update(investment);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<InvestmentDto>(investment);
    }

    public async Task<bool> DeleteInvestmentAsync(Guid id, CancellationToken ct = default)
    {
        var repository = _unitOfWork.Repository<Investment>();
        var investment = await repository.GetByIdAsync(id, ct);
        if (investment == null) return false;

        // Delete associated transactions
        var transactions = await _unitOfWork.Repository<Transaction>().FindAsync(t => t.InvestmentId == id, ct);
        foreach (var tx in transactions)
        {
            _unitOfWork.Repository<Transaction>().Remove(tx);
        }

        repository.Remove(investment);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<CreateAssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken ct = default)
    {
        var asset = _mapper.Map<Asset>(dto);
        await _unitOfWork.Repository<Asset>().AddAsync(asset, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CreateAssetDto>(asset);
    }

    public async Task<Asset?> UpdateAssetAsync(Guid id, CreateAssetDto dto, CancellationToken ct = default)
    {
        var repository = _unitOfWork.Repository<Asset>();
        var asset = await repository.GetByIdAsync(id, ct);
        if (asset == null) return null;

        asset.Name = dto.Name;
        asset.Ticker = string.IsNullOrWhiteSpace(dto.Ticker) ? null : dto.Ticker.ToUpperInvariant();
        asset.AssetCategory = dto.AssetCategory;
        asset.ValuationType = dto.ValuationType;
        asset.Currency = dto.Currency;
        asset.IndexBenchmark = dto.IndexBenchmark;
        asset.LogoUrl = dto.LogoUrl;

        repository.Update(asset);
        await _unitOfWork.SaveChangesAsync(ct);

        return asset;
    }

    public async Task<bool> DeleteAssetAsync(Guid id, CancellationToken ct = default)
    {
        var repository = _unitOfWork.Repository<Asset>();
        var asset = await repository.GetByIdAsync(id, ct);
        if (asset == null) return false;

        repository.Remove(asset);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
