using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.GetSalesPromotion;

public class GetSalesPromotionValidator : AbstractValidator<GetSalesPromotionCommand>
{
    public GetSalesPromotionValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
