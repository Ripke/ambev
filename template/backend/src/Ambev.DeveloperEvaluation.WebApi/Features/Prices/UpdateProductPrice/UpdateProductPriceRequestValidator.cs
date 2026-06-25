using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.UpdateProductPrice;

public class UpdateProductPriceRequestValidator : AbstractValidator<UpdateProductPriceRequest>
{
    public UpdateProductPriceRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PriceType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x).Must(x => x.EffectiveEndAt > x.EffectiveStartAt)
            .WithMessage("EffectiveEndAt must be greater than EffectiveStartAt.");
    }
}
