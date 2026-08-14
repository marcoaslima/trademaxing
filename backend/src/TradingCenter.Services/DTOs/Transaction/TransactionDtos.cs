using TradingCenter.Domain.Enums;

namespace TradingCenter.Services.DTOs.Transaction;

public record TransactionDto(
    Guid Id,
    Guid InvestmentId,
    Guid AccountId,
    TransactionType TransactionType,
    DateTime TransactionDate,
    decimal Quantity,
    decimal PricePerUnit,
    decimal TotalAmount,
    decimal FeeAmount,
    decimal TaxAmount,
    Currency Currency,
    string? Notes,
    DateTime CreatedAt
)
{
    public TransactionDto() : this(Guid.Empty, Guid.Empty, Guid.Empty, default, default, 0, 0, 0, 0, 0, default, null, default) { }
}

public record CreateTransactionDto(
    Guid InvestmentId,
    Guid AccountId,
    TransactionType TransactionType,
    DateTime TransactionDate,
    decimal Quantity,
    decimal PricePerUnit,
    decimal TotalAmount,
    decimal FeeAmount,
    decimal TaxAmount,
    Currency Currency,
    string? Notes
)
{
    public CreateTransactionDto() : this(Guid.Empty, Guid.Empty, default, default, 0, 0, 0, 0, 0, default, null) { }
}
