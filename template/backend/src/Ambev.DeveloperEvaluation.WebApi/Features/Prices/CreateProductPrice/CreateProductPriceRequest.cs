using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.CreateProductPrice;

public class CreateProductPriceRequest
{
    public Guid ProductId { get; set; }
    public PriceType PriceType { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveStartAt { get; set; }
    public DateTime EffectiveEndAt { get; set; }
}
