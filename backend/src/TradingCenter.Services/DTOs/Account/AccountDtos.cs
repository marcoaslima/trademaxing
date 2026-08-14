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
)
{
    public AccountDto() : this(Guid.Empty, Guid.Empty, string.Empty, string.Empty, default, default, default) { }
}

public record CreateAccountDto(
    string Name,
    string Institution,
    AccountType AccountType,
    Currency BaseCurrency
)
{
    public CreateAccountDto() : this(string.Empty, string.Empty, default, default) { }
}
