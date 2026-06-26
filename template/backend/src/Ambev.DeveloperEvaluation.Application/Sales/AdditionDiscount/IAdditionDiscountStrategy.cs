using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;

public interface IAdditionDiscountStrategy
{
    SaleItemAdjustmentKind AdjustmentKind { get; }
    SaleItemAdjustmentType AdjustmentType { get; }

    void Apply(
        SaleItem item,
        decimal value,
        Guid? authorizerId = null,
        string? authorizerName = null,
        string? reason = null);
}
