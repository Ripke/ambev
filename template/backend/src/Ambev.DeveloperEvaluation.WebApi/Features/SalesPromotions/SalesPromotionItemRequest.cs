using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions;

public class SalesPromotionItemRequest
{
    public int MinimumQuantity { get; set; }
    public int MaximumQuantity { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
}
