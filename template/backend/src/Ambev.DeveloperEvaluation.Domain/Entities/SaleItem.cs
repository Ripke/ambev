using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid IdSales { get; private set; }
    public int SequentialNumber { get; private set; }
    public string ProductEan { get; private set; } = string.Empty;
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal AdditionalAmountTotal { get; private set; }
    public decimal DiscountAmountTotal { get; private set; }
    public decimal Total { get; private set; }
    public bool IsCanceled { get; private set; }
    public Guid? CancellationAuthorizerId { get; private set; }
    public string? CancellationAuthorizerName { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime SaleDateTime { get; private set; }
    public List<SalesItemDiscount> Discounts { get; private set; } = [];
    public List<SalesItemAddition> Additions { get; private set; } = [];

    public Sale Sale { get; private set; } = null!;

    public SaleItem()
    {
    }

    public static SaleItem Create(
        Guid saleId,
        int sequentialNumber,
        string productEan,
        Guid productId,
        string productName,
        decimal quantity,
        decimal unitPrice,
        decimal additionalAmountTotal = 0,
        decimal discountAmountTotal = 0)
    {
        var item = new SaleItem
        {
            IdSales = saleId,
            SequentialNumber = sequentialNumber,
            ProductEan = productEan.Trim(),
            ProductId = productId,
            ProductName = productName.Trim(),
            UnitPrice = unitPrice,
            AdditionalAmountTotal = additionalAmountTotal,
            DiscountAmountTotal = discountAmountTotal,
            SaleDateTime = DateTime.UtcNow,
            IsCanceled = false
        };

        item.UpdateQuantity(quantity);
        return item;
    }

    public void UpdateQuantity(decimal quantity)
    {
        if (IsCanceled)
            throw new InvalidOperationException("Canceled item cannot be changed.");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        Quantity = quantity;
        RecalculateTotals();
    }

    public void Cancel(Guid authorizerId, string authorizerName, string? reason)
    {
        if (IsCanceled)
            throw new InvalidOperationException("Item is already canceled.");

        if (authorizerId == Guid.Empty)
            throw new ArgumentException("Cancellation authorizer is required.", nameof(authorizerId));

        if (string.IsNullOrWhiteSpace(authorizerName))
            throw new ArgumentException("Cancellation authorizer name is required.", nameof(authorizerName));

        IsCanceled = true;
        CancellationAuthorizerId = authorizerId;
        CancellationAuthorizerName = authorizerName.Trim();
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    public void ApplyDiscount(
        AdditionDiscountTypes tipoDesconto,
        decimal valor,
        Guid? autorizadorId = null,
        string? autorizadorName = null,
        string? motivo = null)
    {
        EnsureCanChange();
        Discounts.Add(SalesItemDiscount.Create(Id, tipoDesconto, valor, autorizadorId, autorizadorName, motivo));
        RecalculateTotals();
    }

    public void ApplyAddition(
        AdditionDiscountTypes tipo,
        decimal valor,
        Guid? autorizadorId = null,
        string? autorizadorName = null,
        string? motivo = null)
    {
        EnsureCanChange();
        Additions.Add(SalesItemAddition.Create(Id, tipo, valor, autorizadorId, autorizadorName, motivo));
        RecalculateTotals();
    }

    public void RemovePromotionalAdjustments()
    {
        EnsureCanChange();
        Discounts.RemoveAll(discount => discount.TipoDesconto == AdditionDiscountTypes.Promocional);
        Additions.RemoveAll(addition => addition.Tipo == AdditionDiscountTypes.Promocional);
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Subtotal = Quantity * UnitPrice;
        AdditionalAmountTotal = Additions.Sum(addition => addition.Valor);
        DiscountAmountTotal = Discounts.Sum(discount => discount.Valor);
        Total = Subtotal + AdditionalAmountTotal - DiscountAmountTotal;
    }

    private void EnsureCanChange()
    {
        if (IsCanceled)
            throw new InvalidOperationException("Canceled item cannot be changed.");
    }
}
