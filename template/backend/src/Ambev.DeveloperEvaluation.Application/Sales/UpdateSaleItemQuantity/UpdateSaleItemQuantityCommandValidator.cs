using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItemQuantity;

public class UpdateSaleItemQuantityCommandValidator : AbstractValidator<UpdateSaleItemQuantityCommand>
{
    public UpdateSaleItemQuantityCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
