using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.CreateCompany;

public class CreateCompanyCommand : IRequest<CreateCompanyResult>
{
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public CompanyAddressCommand Address { get; set; } = new();
    public CompanyStatus Status { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new CreateCompanyCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
