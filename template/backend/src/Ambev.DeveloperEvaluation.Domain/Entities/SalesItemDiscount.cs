using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SalesItemDiscount : BaseEntity
{
    public Guid IdSalesItem { get; private set; }
    public AdditionDiscountTypes TipoDesconto { get; private set; }
    public decimal Valor { get; private set; }
    public Guid? AutorizadorId { get; private set; }
    public string? AutorizadorName { get; private set; }
    public string? Motivo { get; private set; }
    public DateTime DataHora { get; private set; }

    public SaleItem SaleItem { get; private set; } = null!;

    public SalesItemDiscount()
    {
    }

    public static SalesItemDiscount Create(
        Guid saleItemId,
        AdditionDiscountTypes tipoDesconto,
        decimal valor,
        Guid? autorizadorId = null,
        string? autorizadorName = null,
        string? motivo = null)
    {
        if (saleItemId == Guid.Empty)
            throw new ArgumentException("Sale item ID is required.", nameof(saleItemId));

        if (valor <= 0)
            throw new ArgumentException("Discount value must be greater than zero.", nameof(valor));

        if (tipoDesconto == AdditionDiscountTypes.Manual)
        {
            if (autorizadorId == null || autorizadorId == Guid.Empty)
                throw new ArgumentException("Discount authorizer is required for manual discount.", nameof(autorizadorId));

            if (string.IsNullOrWhiteSpace(autorizadorName))
                throw new ArgumentException("Discount authorizer name is required for manual discount.", nameof(autorizadorName));
        }

        return new SalesItemDiscount
        {
            IdSalesItem = saleItemId,
            TipoDesconto = tipoDesconto,
            Valor = valor,
            AutorizadorId = autorizadorId,
            AutorizadorName = string.IsNullOrWhiteSpace(autorizadorName) ? null : autorizadorName.Trim(),
            Motivo = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim(),
            DataHora = DateTime.UtcNow
        };
    }
}
