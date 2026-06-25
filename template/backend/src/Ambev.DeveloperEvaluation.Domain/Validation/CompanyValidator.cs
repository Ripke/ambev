using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class CompanyValidator : AbstractValidator<Company>
{
    public CompanyValidator()
    {
        RuleFor(company => company.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(company => company.Cnpj)
            .NotEmpty()
            .Must(CnpjHelper.IsValid)
            .WithMessage("The provided CNPJ is not valid.");

        RuleFor(company => company.Status)
            .NotEqual(CompanyStatus.Unknown)
            .WithMessage("Company status cannot be Unknown.");

        RuleFor(company => company.Address)
            .NotNull()
            .SetValidator(new CompanyAddressValidator()!);
    }
}
