using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SalesPromotionItem : BaseEntity
{
    public Guid PromotionId { get; internal set; }
    public int MinimumQuantity { get; private set; }
    public int MaximumQuantity { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public SalesPromotion Promotion { get; private set; } = null!;

    public SalesPromotionItem()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static SalesPromotionItem Create(
        int minimumQuantity,
        int maximumQuantity,
        DiscountType discountType,
        decimal discountValue)
    {
        var item = new SalesPromotionItem
        {
            CreatedAt = DateTime.UtcNow
        };

        item.Update(minimumQuantity, maximumQuantity, discountType, discountValue);
        return item;
    }

    public void Update(
        int minimumQuantity,
        int maximumQuantity,
        DiscountType discountType,
        decimal discountValue)
    {
        ValidateBusinessRules(minimumQuantity, maximumQuantity, discountType, discountValue);

        MinimumQuantity = minimumQuantity;
        MaximumQuantity = maximumQuantity;
        DiscountType = discountType;
        DiscountValue = discountValue;
    }

    public bool MatchesQuantity(decimal quantity)
    {
        return quantity >= MinimumQuantity && quantity <= MaximumQuantity;
    }

    public ValidationResultDetail Validate()
    {
        var validator = new SalesPromotionItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }

    private static void ValidateBusinessRules(
        int minimumQuantity,
        int maximumQuantity,
        DiscountType discountType,
        decimal discountValue)
    {
        if (minimumQuantity <= 0)
            throw new ArgumentException("Minimum quantity must be greater than zero.", nameof(minimumQuantity));

        if (maximumQuantity < minimumQuantity)
            throw new ArgumentException("Maximum quantity must be greater than or equal to minimum quantity.", nameof(maximumQuantity));

        if (!Enum.IsDefined(discountType))
            throw new ArgumentException("Discount type is required.", nameof(discountType));

        if (discountValue <= 0)
            throw new ArgumentException("Discount value must be greater than zero.", nameof(discountValue));
    }
}
