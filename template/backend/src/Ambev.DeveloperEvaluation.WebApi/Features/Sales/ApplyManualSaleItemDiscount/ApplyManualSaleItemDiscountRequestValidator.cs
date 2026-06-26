using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountRequestValidator : AbstractValidator<ApplyManualSaleItemDiscountRequest>
{
    public ApplyManualSaleItemDiscountRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AutorizadorId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
