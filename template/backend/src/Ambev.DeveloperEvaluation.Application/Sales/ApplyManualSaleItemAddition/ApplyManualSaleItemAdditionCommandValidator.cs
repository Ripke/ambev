using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionCommandValidator : AbstractValidator<ApplyManualSaleItemAdditionCommand>
{
    public ApplyManualSaleItemAdditionCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AuthorizerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
