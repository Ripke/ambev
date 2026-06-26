using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.DeleteSalesPromotion;

public class DeleteSalesPromotionRequestValidator : AbstractValidator<DeleteSalesPromotionRequest>
{
    public DeleteSalesPromotionRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
