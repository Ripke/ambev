namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

public class CancelSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public Guid CancellationAuthorizerId { get; set; }
    public string? CancellationReason { get; set; }
}
