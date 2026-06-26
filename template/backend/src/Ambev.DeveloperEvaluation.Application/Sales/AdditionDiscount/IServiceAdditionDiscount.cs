using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;

public interface IServiceAdditionDiscount
{
    Task<Sale> Apply(
        SaleItemAdjustmentKind operationKind,
        SaleItemAdjustmentType adjustmentType,
        Guid saleId,
        Guid saleItemId,
        decimal amount,
        Guid? authorizerId = null,
        string? reason = null,
        CancellationToken cancellationToken = default);
}
