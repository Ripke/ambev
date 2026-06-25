using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;

public class ReopenSaleCommandValidator : AbstractValidator<ReopenSaleCommand>
{
    public ReopenSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}
