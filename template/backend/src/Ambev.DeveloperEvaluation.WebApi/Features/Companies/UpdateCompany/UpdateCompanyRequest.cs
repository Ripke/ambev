using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.UpdateCompany;

public class UpdateCompanyRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public CompanyAddressRequest Address { get; set; } = new();
    public CompanyStatus Status { get; set; }
}
