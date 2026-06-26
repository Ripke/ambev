using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;

public class DiscountPromotionalStrategy : IAdditionDiscountStrategy
{
    public SaleItemAdjustmentKind AdjustmentKind => SaleItemAdjustmentKind.Discount;
    public SaleItemAdjustmentType AdjustmentType => SaleItemAdjustmentType.Promotional;

    public void Apply(
        SaleItem item,
        decimal value,
        Guid? authorizerId = null,
        string? authorizerName = null,
        string? reason = null)
    {
        item.ApplyDiscount(SaleItemAdjustmentType.Promotional, value, authorizerId, authorizerName, reason);
    }
}
