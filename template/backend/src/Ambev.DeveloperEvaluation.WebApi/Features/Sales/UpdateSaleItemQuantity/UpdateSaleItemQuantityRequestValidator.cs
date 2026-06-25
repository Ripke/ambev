using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItemQuantity;

public class UpdateSaleItemQuantityRequestValidator : AbstractValidator<UpdateSaleItemQuantityRequest>
{
    public UpdateSaleItemQuantityRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
