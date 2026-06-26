using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SalesPromotionValidator : AbstractValidator<SalesPromotion>
{
    public SalesPromotionValidator()
    {
        RuleFor(promotion => promotion.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(promotion => promotion.Description)
            .MaximumLength(1000);

        RuleFor(promotion => promotion.Priority)
            .GreaterThanOrEqualTo(0);

        RuleFor(promotion => promotion.StartDate)
            .NotEmpty();

        RuleFor(promotion => promotion.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(promotion => promotion.StartDate);

        RuleFor(promotion => promotion.Items)
            .NotEmpty();

        RuleForEach(promotion => promotion.Items)
            .SetValidator(new SalesPromotionItemValidator());

        RuleFor(promotion => promotion.Items)
            .Must(HaveNoOverlappingRanges)
            .WithMessage("Promotion items cannot contain overlapping quantity ranges.");
    }

    private static bool HaveNoOverlappingRanges(IReadOnlyCollection<SalesPromotionItem> items)
    {
        var orderedItems = items.OrderBy(item => item.MinimumQuantity).ToList();
        for (var i = 1; i < orderedItems.Count; i++)
        {
            if (orderedItems[i].MinimumQuantity <= orderedItems[i - 1].MaximumQuantity)
                return false;
        }

        return true;
    }
}
