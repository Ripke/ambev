namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public class CancelSaleRequest
{
    public Guid Id { get; set; }
    public Guid Version { get; set; }
    public Guid CancellationAuthorizerId { get; set; }
    public string? CancellationReason { get; set; }
}
