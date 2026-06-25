using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByCustomer;

public class GetCurrentSaleByCustomerValidator : AbstractValidator<GetCurrentSaleByCustomerCommand>
{
    public GetCurrentSaleByCustomerValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
