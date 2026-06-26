using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SalesItemAddition : BaseEntity
{
    public Guid IdSalesItem { get; private set; }
    public AdditionDiscountTypes Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public Guid? AutorizadorId { get; private set; }
    public string? AutorizadorName { get; private set; }
    public string? Motivo { get; private set; }
    public DateTime DataHora { get; private set; }

    public SaleItem SaleItem { get; private set; } = null!;

    public SalesItemAddition()
    {
    }

    public static SalesItemAddition Create(
        Guid saleItemId,
        AdditionDiscountTypes tipo,
        decimal valor,
        Guid? autorizadorId = null,
        string? autorizadorName = null,
        string? motivo = null)
    {
        if (saleItemId == Guid.Empty)
            throw new ArgumentException("Sale item ID is required.", nameof(saleItemId));

        if (valor <= 0)
            throw new ArgumentException("Addition value must be greater than zero.", nameof(valor));

        if (tipo == AdditionDiscountTypes.Manual)
        {
            if (autorizadorId == null || autorizadorId == Guid.Empty)
                throw new ArgumentException("Addition authorizer is required for manual addition.", nameof(autorizadorId));

            if (string.IsNullOrWhiteSpace(autorizadorName))
                throw new ArgumentException("Addition authorizer name is required for manual addition.", nameof(autorizadorName));
        }

        return new SalesItemAddition
        {
            IdSalesItem = saleItemId,
            Tipo = tipo,
            Valor = valor,
            AutorizadorId = autorizadorId,
            AutorizadorName = string.IsNullOrWhiteSpace(autorizadorName) ? null : autorizadorName.Trim(),
            Motivo = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim(),
            DataHora = DateTime.UtcNow
        };
    }
}
