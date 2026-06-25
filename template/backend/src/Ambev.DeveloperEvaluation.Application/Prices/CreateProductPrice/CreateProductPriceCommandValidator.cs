using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Prices.CreateProductPrice;

public class CreateProductPriceCommandValidator : AbstractValidator<CreateProductPriceCommand>
{
    public CreateProductPriceCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.PriceType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.EffectiveStartAt).NotEmpty();
        RuleFor(x => x.EffectiveEndAt).NotEmpty();
        RuleFor(x => x).Must(x => x.EffectiveEndAt > x.EffectiveStartAt)
            .WithMessage("EffectiveEndAt must be greater than EffectiveStartAt.");
    }
}
