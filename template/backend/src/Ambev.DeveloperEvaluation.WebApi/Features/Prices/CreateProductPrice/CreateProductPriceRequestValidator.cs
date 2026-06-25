using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.CreateProductPrice;

public class CreateProductPriceRequestValidator : AbstractValidator<CreateProductPriceRequest>
{
    public CreateProductPriceRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.PriceType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x).Must(x => x.EffectiveEndAt > x.EffectiveStartAt)
            .WithMessage("EffectiveEndAt must be greater than EffectiveStartAt.");
    }
}
