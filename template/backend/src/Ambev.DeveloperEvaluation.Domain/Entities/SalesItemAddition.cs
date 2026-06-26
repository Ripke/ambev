using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SalesItemAddition : BaseEntity
{
    public Guid SaleItemId { get; private set; }
    public SaleItemAdjustmentType AdjustmentType { get; private set; }
    public decimal Amount { get; private set; }
    public Guid? AuthorizerId { get; private set; }
    public string? AuthorizerName { get; private set; }
    public string? Reason { get; private set; }
    public DateTime OccurredAt { get; private set; }

    public SaleItem SaleItem { get; private set; } = null!;

    public SalesItemAddition()
    {
    }

    public static SalesItemAddition Create(
        Guid saleItemId,
        SaleItemAdjustmentType tipo,
        decimal valor,
        Guid? autorizadorId = null,
        string? autorizadorName = null,
        string? motivo = null)
    {
        if (saleItemId == Guid.Empty)
            throw new ArgumentException("Sale item ID is required.", nameof(saleItemId));

        if (valor <= 0)
            throw new ArgumentException("Addition value must be greater than zero.", nameof(valor));

        if (tipo == SaleItemAdjustmentType.Manual)
        {
            if (autorizadorId == null || autorizadorId == Guid.Empty)
                throw new ArgumentException("Addition authorizer is required for manual addition.", nameof(autorizadorId));

            if (string.IsNullOrWhiteSpace(autorizadorName))
                throw new ArgumentException("Addition authorizer name is required for manual addition.", nameof(autorizadorName));
        }

        return new SalesItemAddition
        {
            SaleItemId = saleItemId,
            AdjustmentType = tipo,
            Amount = valor,
            AuthorizerId = autorizadorId,
            AuthorizerName = string.IsNullOrWhiteSpace(autorizadorName) ? null : autorizadorName.Trim(),
            Reason = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim(),
            OccurredAt = DateTime.UtcNow
        };
    }
}
