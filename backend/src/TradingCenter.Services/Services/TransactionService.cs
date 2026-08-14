using AutoMapper;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Transaction;

namespace TradingCenter.Services.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(Guid? investmentId = null, CancellationToken ct = default);
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken ct = default);
    Task<TransactionDto?> UpdateTransactionAsync(Guid id, CreateTransactionDto dto, CancellationToken ct = default);
    Task<bool> DeleteTransactionAsync(Guid id, CancellationToken ct = default);
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

    public async Task<TransactionDto?> UpdateTransactionAsync(Guid id, CreateTransactionDto dto, CancellationToken ct = default)
    {
        var repository = _unitOfWork.Repository<Transaction>();
        var tx = await repository.GetByIdAsync(id, ct);
        if (tx == null) return null;

        tx.InvestmentId = dto.InvestmentId;
        tx.AccountId = dto.AccountId;
        tx.TransactionType = dto.TransactionType;
        tx.TransactionDate = dto.TransactionDate;
        tx.Quantity = dto.Quantity;
        tx.PricePerUnit = dto.PricePerUnit;
        tx.TotalAmount = dto.TotalAmount;
        tx.FeeAmount = dto.FeeAmount;
        tx.TaxAmount = dto.TaxAmount;
        tx.Currency = dto.Currency;
        tx.Notes = dto.Notes;

        repository.Update(tx);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<TransactionDto>(tx);
    }

    public async Task<bool> DeleteTransactionAsync(Guid id, CancellationToken ct = default)
    {
        var repository = _unitOfWork.Repository<Transaction>();
        var tx = await repository.GetByIdAsync(id, ct);
        if (tx == null) return false;

        repository.Remove(tx);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
