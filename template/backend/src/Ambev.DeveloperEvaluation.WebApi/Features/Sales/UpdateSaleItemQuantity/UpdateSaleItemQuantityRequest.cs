namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItemQuantity;

public class UpdateSaleItemQuantityRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
}
