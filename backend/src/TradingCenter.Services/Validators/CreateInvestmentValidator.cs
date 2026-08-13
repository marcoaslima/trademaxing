using FluentValidation;
using TradingCenter.Services.DTOs.Investment;

namespace TradingCenter.Services.Validators;

public class CreateInvestmentValidator : AbstractValidator<CreateInvestmentDto>
{
    public CreateInvestmentValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");

        RuleFor(x => x.AssetId)
            .NotEmpty().WithMessage("Asset ID is required.");
    }
}
