using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionRequestValidator : AbstractValidator<ApplyManualSaleItemAdditionRequest>
{
    public ApplyManualSaleItemAdditionRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.AutorizadorId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
