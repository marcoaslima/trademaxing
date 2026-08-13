using AutoMapper;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Transaction;

namespace TradingCenter.Services.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(Guid? investmentId = null, CancellationToken ct = default);
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken ct = default);
}

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(Guid? investmentId = null, CancellationToken ct = default)
    {
        var list = investmentId.HasValue
            ? await _unitOfWork.Repository<Transaction>().FindAsync(t => t.InvestmentId == investmentId.Value, ct)
            : await _unitOfWork.Repository<Transaction>().GetAllAsync(ct);

        return _mapper.Map<IReadOnlyList<TransactionDto>>(list);
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken ct = default)
    {
        var transaction = _mapper.Map<Transaction>(dto);
        await _unitOfWork.Repository<Transaction>().AddAsync(transaction, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<TransactionDto>(transaction);
    }
}
