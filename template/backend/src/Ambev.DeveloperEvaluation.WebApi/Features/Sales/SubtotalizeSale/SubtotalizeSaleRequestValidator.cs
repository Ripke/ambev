using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.SubtotalizeSale;

public class SubtotalizeSaleRequestValidator : AbstractValidator<SubtotalizeSaleRequest>
{
    public SubtotalizeSaleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}
