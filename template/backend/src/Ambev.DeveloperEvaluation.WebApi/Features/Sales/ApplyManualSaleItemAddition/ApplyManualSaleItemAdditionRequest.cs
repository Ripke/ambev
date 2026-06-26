namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Valor { get; set; }
    public Guid AutorizadorId { get; set; }
    public string? Motivo { get; set; }
}
