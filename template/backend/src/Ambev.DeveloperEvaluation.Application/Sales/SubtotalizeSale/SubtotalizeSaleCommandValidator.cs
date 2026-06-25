using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.SubtotalizeSale;

public class SubtotalizeSaleCommandValidator : AbstractValidator<SubtotalizeSaleCommand>
{
    public SubtotalizeSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}
