namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;

public class AddSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid? ProductId { get; set; }
    public string? Ean { get; set; }
    public decimal Quantity { get; set; }
}
