using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.UpdateSalesPromotion;

public class UpdateSalesPromotionRequestValidator : AbstractValidator<UpdateSalesPromotionRequest>
{
    public UpdateSalesPromotionRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Priority).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new SalesPromotionItemRequestValidator());
    }
}
