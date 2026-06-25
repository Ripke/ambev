using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommandValidator : AbstractValidator<CancelSaleCommand>
{
    public CancelSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
        RuleFor(x => x.CancellationAuthorizerId).NotEmpty();
        RuleFor(x => x.CancellationReason).MaximumLength(500);
    }
}
