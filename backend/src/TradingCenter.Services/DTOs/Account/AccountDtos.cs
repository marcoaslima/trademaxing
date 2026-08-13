using TradingCenter.Domain.Enums;

namespace TradingCenter.Services.DTOs.Account;

public record AccountDto(
    Guid Id,
    Guid UserId,
    string Name,
    string Institution,
    AccountType AccountType,
    Currency BaseCurrency,
    DateTime CreatedAt
);

public record CreateAccountDto(
    string Name,
    string Institution,
    AccountType AccountType,
    Currency BaseCurrency
);
