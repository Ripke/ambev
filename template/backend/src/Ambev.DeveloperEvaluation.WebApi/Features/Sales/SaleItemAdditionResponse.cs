using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SaleItemAdditionResponse
{
    public Guid Id { get; set; }
    public Guid IdSalesItem { get; set; }
    public AdditionDiscountTypes Tipo { get; set; }
    public decimal Valor { get; set; }
    public Guid? AutorizadorId { get; set; }
    public string? AutorizadorName { get; set; }
    public string? Motivo { get; set; }
    public DateTime DataHora { get; set; }
}
