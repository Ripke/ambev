using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions;

public class SalesPromotionItemInput
{
    public int MinimumQuantity { get; set; }
    public int MaximumQuantity { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
}
