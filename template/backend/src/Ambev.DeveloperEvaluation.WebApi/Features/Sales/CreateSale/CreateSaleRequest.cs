namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
}
