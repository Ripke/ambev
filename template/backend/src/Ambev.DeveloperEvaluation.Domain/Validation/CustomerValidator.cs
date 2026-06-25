using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(customer => customer.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(customer => customer.BirthDate)
            .Must(date => date.Date <= DateTime.UtcNow.Date)
            .WithMessage("Birth date cannot be in the future.");

        RuleFor(customer => customer.EncryptedCpf)
            .NotEmpty()
            .WithMessage("Encrypted CPF is required.");

        RuleFor(customer => customer.Status)
            .NotEqual(CustomerStatus.Unknown)
            .WithMessage("Customer status cannot be Unknown.");
    }
}
