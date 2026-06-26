namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Amount { get; set; }
    public Guid AuthorizerId { get; set; }
    public string? Reason { get; set; }
}
