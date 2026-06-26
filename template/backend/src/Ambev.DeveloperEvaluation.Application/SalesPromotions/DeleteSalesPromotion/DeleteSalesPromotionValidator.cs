using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.DeleteSalesPromotion;

public class DeleteSalesPromotionValidator : AbstractValidator<DeleteSalesPromotionCommand>
{
    public DeleteSalesPromotionValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
