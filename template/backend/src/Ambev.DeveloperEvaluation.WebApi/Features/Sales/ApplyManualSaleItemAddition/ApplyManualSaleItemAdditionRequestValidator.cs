using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionRequestValidator : AbstractValidator<ApplyManualSaleItemAdditionRequest>
{
    public ApplyManualSaleItemAdditionRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AuthorizerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
