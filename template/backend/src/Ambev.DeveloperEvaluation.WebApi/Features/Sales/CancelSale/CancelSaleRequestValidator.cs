using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public class CancelSaleRequestValidator : AbstractValidator<CancelSaleRequest>
{
    public CancelSaleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
        RuleFor(x => x.CancellationAuthorizerId).NotEmpty();
        RuleFor(x => x.CancellationReason).MaximumLength(500);
    }
}
