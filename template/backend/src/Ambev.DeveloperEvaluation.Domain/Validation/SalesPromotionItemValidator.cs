using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SalesPromotionItemValidator : AbstractValidator<SalesPromotionItem>
{
    public SalesPromotionItemValidator()
    {
        RuleFor(item => item.MinimumQuantity)
            .GreaterThan(0);

        RuleFor(item => item.MaximumQuantity)
            .GreaterThanOrEqualTo(item => item.MinimumQuantity);

        RuleFor(item => item.DiscountType)
            .IsInEnum()
            .Must(type => type != 0)
            .WithMessage("Discount type is required.");

        RuleFor(item => item.DiscountValue)
            .GreaterThan(0);
    }
}
