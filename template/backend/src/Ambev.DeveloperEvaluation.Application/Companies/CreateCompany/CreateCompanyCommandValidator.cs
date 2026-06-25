using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Companies.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cnpj).SetValidator(new CnpjValidator());
        RuleFor(x => x.Address).NotNull().SetValidator(new CompanyAddressCommandValidator()!);
        RuleFor(x => x.Status)
            .IsInEnum()
            .Must(status => status != CompanyStatus.Unknown);
    }
}
