using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.CreateCompany;

public class CreateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public CompanyAddressRequest Address { get; set; } = new();
    public CompanyStatus Status { get; set; }
}
