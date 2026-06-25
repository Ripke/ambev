using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Customers.GetCustomer;

public class GetCustomerValidator : AbstractValidator<GetCustomerCommand>
{
    public GetCustomerValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
