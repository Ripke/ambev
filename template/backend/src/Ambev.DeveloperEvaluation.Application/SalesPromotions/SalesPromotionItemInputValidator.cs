using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions;

public class SalesPromotionItemInputValidator : AbstractValidator<SalesPromotionItemInput>
{
    public SalesPromotionItemInputValidator()
    {
        RuleFor(x => x.MinimumQuantity).GreaterThan(0);
        RuleFor(x => x.MaximumQuantity).GreaterThanOrEqualTo(x => x.MinimumQuantity);
        RuleFor(x => x.DiscountType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.DiscountValue).GreaterThan(0);
    }
}
