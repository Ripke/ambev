using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Prices.UpdateProductPrice;

public class UpdateProductPriceResult
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public PriceType PriceType { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveStartAt { get; set; }
    public DateTime EffectiveEndAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
