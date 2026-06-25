using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.CreateCustomer;

public class CreateCustomerRequest
{
    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public CustomerStatus Status { get; set; }
}
