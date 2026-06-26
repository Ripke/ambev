using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions;

public class SalesPromotionItemRequestValidator : AbstractValidator<SalesPromotionItemRequest>
{
    public SalesPromotionItemRequestValidator()
    {
        RuleFor(x => x.MinimumQuantity).GreaterThan(0);
        RuleFor(x => x.MaximumQuantity).GreaterThanOrEqualTo(x => x.MinimumQuantity);
        RuleFor(x => x.DiscountType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.DiscountValue).GreaterThan(0);
    }
}
