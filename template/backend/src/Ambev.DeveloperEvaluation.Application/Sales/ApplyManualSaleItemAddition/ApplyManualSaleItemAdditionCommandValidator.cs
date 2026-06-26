using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionCommandValidator : AbstractValidator<ApplyManualSaleItemAdditionCommand>
{
    public ApplyManualSaleItemAdditionCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AutorizadorId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
