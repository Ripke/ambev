using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountRequestValidator : AbstractValidator<ApplyManualSaleItemDiscountRequest>
{
    public ApplyManualSaleItemDiscountRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AuthorizerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
