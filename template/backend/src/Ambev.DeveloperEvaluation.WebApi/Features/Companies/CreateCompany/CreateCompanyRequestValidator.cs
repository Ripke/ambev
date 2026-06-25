using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.CreateCompany;

public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cnpj).SetValidator(new CnpjValidator());
        RuleFor(x => x.Address).NotNull().SetValidator(new CompanyAddressRequestValidator()!);
        RuleFor(x => x.Status)
            .IsInEnum()
            .Must(status => status != CompanyStatus.Unknown);
    }
}
