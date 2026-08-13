using FluentValidation;
using TradingCenter.Services.DTOs.Transaction;

namespace TradingCenter.Services.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.InvestmentId)
            .NotEmpty().WithMessage("Investment ID is required.");

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

        RuleFor(x => x.PricePerUnit)
            .GreaterThanOrEqualTo(0).WithMessage("Price per unit cannot be negative.");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative.");
    }
}
