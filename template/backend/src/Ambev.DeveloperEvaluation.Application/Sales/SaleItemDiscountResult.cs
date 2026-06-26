using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleItemDiscountResult
{
    public Guid Id { get; set; }
    public Guid SaleItemId { get; set; }
    public SaleItemAdjustmentType AdjustmentType { get; set; }
    public decimal Amount { get; set; }
    public Guid? AuthorizerId { get; set; }
    public string? AuthorizerName { get; set; }
    public string? Reason { get; set; }
    public DateTime OccurredAt { get; set; }
}
