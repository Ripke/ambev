using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountCommandValidator : AbstractValidator<ApplyManualSaleItemDiscountCommand>
{
    public ApplyManualSaleItemDiscountCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AuthorizerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
