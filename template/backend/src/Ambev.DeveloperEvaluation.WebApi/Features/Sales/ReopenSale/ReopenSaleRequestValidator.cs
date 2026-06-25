using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ReopenSale;

public class ReopenSaleRequestValidator : AbstractValidator<ReopenSaleRequest>
{
    public ReopenSaleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}
