using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.GetSalesPromotion;

public class GetSalesPromotionRequestValidator : AbstractValidator<GetSalesPromotionRequest>
{
    public GetSalesPromotionRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
