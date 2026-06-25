namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SaleItemResponse
{
    public Guid IdSales { get; set; }
    public Guid Id { get; set; }
    public int SequentialNumber { get; set; }
    public string ProductEan { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public decimal AdditionalAmountTotal { get; set; }
    public decimal DiscountAmountTotal { get; set; }
    public decimal Total { get; set; }
    public bool IsCanceled { get; set; }
    public Guid? CancellationAuthorizerId { get; set; }
    public string? CancellationAuthorizerName { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime SaleDateTime { get; set; }
}
