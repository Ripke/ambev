using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByUser;

public class GetCurrentSaleByUserValidator : AbstractValidator<GetCurrentSaleByUserCommand>
{
    public GetCurrentSaleByUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
