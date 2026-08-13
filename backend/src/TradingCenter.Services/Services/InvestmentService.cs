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
        var list = accountId.HasValue 
            ? await _unitOfWork.Repository<Investment>().FindAsync(i => i.AccountId == accountId.Value, ct)
            : await _unitOfWork.Repository<Investment>().GetAllAsync(ct);

        return _mapper.Map<IReadOnlyList<InvestmentDto>>(list);
    }

    public async Task<InvestmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _unitOfWork.Repository<Investment>().GetByIdAsync(id, ct);
        return item == null ? null : _mapper.Map<InvestmentDto>(item);
    }

    public async Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentDto dto, CancellationToken ct = default)
    {
        var investment = _mapper.Map<Investment>(dto);
        await _unitOfWork.Repository<Investment>().AddAsync(investment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<InvestmentDto>(investment);
    }
}
