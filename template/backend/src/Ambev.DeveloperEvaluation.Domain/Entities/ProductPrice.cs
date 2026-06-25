using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class ProductPrice : BaseEntity
{
    public Guid ProductId { get; private set; }
    public PriceType PriceType { get; private set; }
    public decimal Price { get; private set; }
    public DateTime EffectiveStartAt { get; private set; }
    public DateTime EffectiveEndAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Product Product { get; private set; } = null!;

    public ProductPrice()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static ProductPrice Create(
        Guid productId,
        PriceType priceType,
        decimal price,
        DateTime effectiveStartAt,
        DateTime effectiveEndAt)
    {
        var productPrice = new ProductPrice
        {
            ProductId = productId,
            CreatedAt = DateTime.UtcNow
        };

        productPrice.Define(priceType, price, effectiveStartAt, effectiveEndAt);
        productPrice.AddEvent(new ProductPriceCreatedEvent(productId, priceType, price, effectiveStartAt, effectiveEndAt));
        return productPrice;
    }

    public void Update(PriceType priceType, decimal price, DateTime effectiveStartAt, DateTime effectiveEndAt)
    {
        Define(priceType, price, effectiveStartAt, effectiveEndAt);
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new ProductPriceUpdatedEvent(ProductId, priceType, price, UpdatedAt.Value));
    }

    public void MarkAsRemoved()
    {
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new ProductPriceRemovedEvent(ProductId, PriceType, UpdatedAt.Value));
    }

    private void Define(PriceType priceType, decimal price, DateTime effectiveStartAt, DateTime effectiveEndAt)
    {
        PriceType = priceType;
        Price = price;
        EffectiveStartAt = effectiveStartAt;
        EffectiveEndAt = effectiveEndAt;
    }
}
