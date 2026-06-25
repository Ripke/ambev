using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetCurrentSaleByCustomer;

public class GetCurrentSaleByCustomerRequestValidator : AbstractValidator<GetCurrentSaleByCustomerRequest>
{
    public GetCurrentSaleByCustomerRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
