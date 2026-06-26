using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountCommandValidator : AbstractValidator<ApplyManualSaleItemDiscountCommand>
{
    public ApplyManualSaleItemDiscountCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AutorizadorId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
