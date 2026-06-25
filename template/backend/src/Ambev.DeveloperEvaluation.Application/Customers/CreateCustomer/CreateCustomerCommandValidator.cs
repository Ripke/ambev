using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Customers.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
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
