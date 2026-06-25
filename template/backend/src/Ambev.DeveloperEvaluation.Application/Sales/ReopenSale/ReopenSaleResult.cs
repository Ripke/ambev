using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;

public class ReopenSaleResult
{
    public Guid Id { get; set; }
    public long SaleNumber { get; set; }
    public Guid Version { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime MovementDate { get; set; }
    public DateTime? FinishedAt { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public decimal AdditionalAmountTotal { get; set; }
    public decimal DiscountAmountTotal { get; set; }
    public decimal PaymentAmountTotal { get; set; }
    public decimal ChangeAmountTotal { get; set; }
    public SaleStatus Status { get; set; }
    public DateTime StatusChangedDate { get; set; }
    public bool IsCanceled { get; set; }
    public Guid? CancellationAuthorizerId { get; set; }
    public string? CancellationAuthorizerName { get; set; }
    public string? CancellationReason { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
