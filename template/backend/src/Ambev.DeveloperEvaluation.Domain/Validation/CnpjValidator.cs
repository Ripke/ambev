using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class CnpjValidator : AbstractValidator<string>
{
    public CnpjValidator()
    {
        RuleFor(cnpj => cnpj)
            .NotEmpty()
            .Must(CnpjHelper.IsValid)
            .WithMessage("The provided CNPJ is not valid.");
    }
}
