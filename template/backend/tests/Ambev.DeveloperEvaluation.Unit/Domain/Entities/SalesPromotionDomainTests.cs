using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SalesPromotionDomainTests
{
    [Fact]
    public void Create_WithGlobalPromotionAndValidRanges_CreatesPromotion()
    {
        var promotion = SalesPromotion.Create(
            "Promo",
            "Descricao",
            1,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            null,
            true,
            [
                SalesPromotionItem.Create(1, 3, DiscountType.Percentage, 10),
                SalesPromotionItem.Create(4, 9, DiscountType.FixedAmount, 5)
            ]);

        promotion.ProductId.Should().BeNull();
        promotion.Items.Should().HaveCount(2);
        promotion.Validate().IsValid.Should().BeTrue();
    }

    [Fact]
    public void Create_WithSpecificProductPromotion_CreatesPromotion()
    {
        var productId = Guid.NewGuid();

        var promotion = SalesPromotion.Create(
            "Promo",
            null,
            0,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            productId,
            true,
            [SalesPromotionItem.Create(4, 9, DiscountType.Percentage, 10)]);

        promotion.ProductId.Should().Be(productId);
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_Throws()
    {
        var act = () => SalesPromotion.Create(
            "Promo",
            null,
            0,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            [SalesPromotionItem.Create(4, 9, DiscountType.Percentage, 10)]);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNegativePriority_Throws()
    {
        var act = () => SalesPromotion.Create(
            "Promo",
            null,
            -1,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            null,
            true,
            [SalesPromotionItem.Create(4, 9, DiscountType.Percentage, 10)]);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void PromotionItem_WithMinimumQuantityLessThanOrEqualToZero_Throws()
    {
        var act = () => SalesPromotionItem.Create(0, 9, DiscountType.Percentage, 10);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void PromotionItem_WithMaximumQuantityLessThanMinimum_Throws()
    {
        var act = () => SalesPromotionItem.Create(10, 9, DiscountType.Percentage, 10);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void PromotionItem_WithDiscountValueLessThanOrEqualToZero_Throws()
    {
        var act = () => SalesPromotionItem.Create(1, 9, DiscountType.Percentage, 0);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithOverlappingRanges_Throws()
    {
        var act = () => SalesPromotion.Create(
            "Promo",
            null,
            0,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            null,
            true,
            [
                SalesPromotionItem.Create(1, 5, DiscountType.Percentage, 10),
                SalesPromotionItem.Create(4, 10, DiscountType.Percentage, 20)
            ]);

        act.Should().Throw<ArgumentException>();
    }
}
