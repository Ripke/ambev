namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Amount { get; set; }
    public Guid AuthorizerId { get; set; }
    public string? Reason { get; set; }
}
