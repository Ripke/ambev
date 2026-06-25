using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class ProductBarcode : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Barcode { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Product Product { get; private set; } = null!;

    public ProductBarcode()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static ProductBarcode Create(Guid productId, string barcode)
    {
        return new ProductBarcode
        {
            ProductId = productId,
            Barcode = barcode.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRemoved()
    {
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new ProductBarcodeRemovedEvent(ProductId, Barcode, UpdatedAt.Value));
    }
}
