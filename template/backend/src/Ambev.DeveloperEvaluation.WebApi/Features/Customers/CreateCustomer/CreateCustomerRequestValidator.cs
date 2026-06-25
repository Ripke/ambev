using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.CreateCustomer;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BirthDate)
            .Must(date => date.Date <= DateTime.UtcNow.Date)
            .WithMessage("Birth date cannot be in the future.");
        RuleFor(x => x.Cpf).SetValidator(new CpfValidator());
        RuleFor(x => x.Status)
            .IsInEnum()
            .Must(status => status != CustomerStatus.Unknown);
    }
}
