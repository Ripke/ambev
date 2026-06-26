using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleItemDiscountResult
{
    public Guid Id { get; set; }
    public Guid IdSalesItem { get; set; }
    public AdditionDiscountTypes TipoDesconto { get; set; }
    public decimal Valor { get; set; }
    public Guid? AutorizadorId { get; set; }
    public string? AutorizadorName { get; set; }
    public string? Motivo { get; set; }
    public DateTime DataHora { get; set; }
}
