using AutoMapper;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Investment;

namespace TradingCenter.Services.Services;

public interface IInvestmentService
{
    Task<IReadOnlyList<InvestmentDto>> GetInvestmentsAsync(Guid? accountId = null, CancellationToken ct = default);
    Task<InvestmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentDto dto, CancellationToken ct = default);
    Task<CreateAssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken ct = default);
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
            ?? throw new ArgumentException("Master Asset not found.");

        var investment = _mapper.Map<Investment>(dto);
        investment.Asset = asset;

        await _unitOfWork.Repository<Investment>().AddAsync(investment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<InvestmentDto>(investment);
    }

    public async Task<CreateAssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken ct = default)
    {
        var asset = _mapper.Map<Asset>(dto);
        await _unitOfWork.Repository<Asset>().AddAsync(asset, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CreateAssetDto>(asset);
    }
}
