namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Valor { get; set; }
    public Guid AutorizadorId { get; set; }
    public string? Motivo { get; set; }
}
