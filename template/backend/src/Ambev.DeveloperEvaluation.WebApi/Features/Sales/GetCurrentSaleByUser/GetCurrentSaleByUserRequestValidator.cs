using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetCurrentSaleByUser;

public class GetCurrentSaleByUserRequestValidator : AbstractValidator<GetCurrentSaleByUserRequest>
{
    public GetCurrentSaleByUserRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
