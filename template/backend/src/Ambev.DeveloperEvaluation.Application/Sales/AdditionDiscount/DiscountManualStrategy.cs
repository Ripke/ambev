using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;

public class DiscountManualStrategy : IAdditionDiscountStrategy
{
    public SaleItemAdjustmentKind AdjustmentKind => SaleItemAdjustmentKind.Discount;
    public SaleItemAdjustmentType AdjustmentType => SaleItemAdjustmentType.Manual;

    public void Apply(
        SaleItem item,
        decimal value,
        Guid? authorizerId = null,
        string? authorizerName = null,
        string? reason = null)
    {
        item.ApplyDiscount(SaleItemAdjustmentType.Manual, value, authorizerId, authorizerName, reason);
    }
}
