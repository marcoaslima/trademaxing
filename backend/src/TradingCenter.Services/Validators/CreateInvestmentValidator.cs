using FluentValidation;
using TradingCenter.Services.DTOs.Investment;

namespace TradingCenter.Services.Validators;

public class CreateInvestmentValidator : AbstractValidator<CreateInvestmentDto>
{
    public CreateInvestmentValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Investment name is required.")
            .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

        RuleFor(x => x.AssetCategory)
            .IsInEnum().WithMessage("Valid asset category is required.");

        RuleFor(x => x.ValuationType)
            .IsInEnum().WithMessage("Valid valuation type is required.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Valid currency is required.");
    }
}
