using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Prices.UpdateProductPrice;

public class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PriceType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x).Must(x => x.EffectiveEndAt > x.EffectiveStartAt)
            .WithMessage("EffectiveEndAt must be greater than EffectiveStartAt.");
    }
}
