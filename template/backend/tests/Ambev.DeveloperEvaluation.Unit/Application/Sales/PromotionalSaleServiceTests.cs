using Ambev.DeveloperEvaluation.Application.Sales.Promotions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class PromotionalSaleServiceTests
{
    [Fact]
    public async Task ApplyAsync_WithoutApplicablePromotion_DoesNotAddAdjustments()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 3, 10);

        repository.GetApplicableItemAsync(item.ProductId, item.Quantity, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((SalesPromotionItem?)null);

        await service.ApplyAsync(sale, CancellationToken.None);

        item.Discounts.Should().BeEmpty();
        item.Additions.Should().BeEmpty();
        sale.Total.Should().Be(30);
    }

    [Fact]
    public async Task ApplyAsync_WithPercentagePromotion_AppliesExpectedDiscount()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 4, 10);
        var promotion = SalesPromotion.Create("Promo", null, 1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, true, [SalesPromotionItem.Create(4, 9, DiscountType.Percentage, 10)]);
        var promotionItem = promotion.Items.Single();

        repository.GetApplicableItemAsync(item.ProductId, item.Quantity, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        item.DiscountAmountTotal.Should().Be(4);
        item.Total.Should().Be(36);
    }

    [Fact]
    public async Task ApplyAsync_WithFixedAmountPromotion_AppliesExpectedDiscount()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 10, 10);
        var promotion = SalesPromotion.Create("Promo", null, 1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, true, [SalesPromotionItem.Create(10, 20, DiscountType.FixedAmount, 2)]);
        var promotionItem = promotion.Items.Single();

        repository.GetApplicableItemAsync(item.ProductId, item.Quantity, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        item.DiscountAmountTotal.Should().Be(20);
        item.Total.Should().Be(80);
    }

    [Fact]
    public async Task ApplyAsync_WithFixedPriceLowerThanUnitPrice_AppliesDiscount()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var promotion = SalesPromotion.Create("Promo", null, 1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, true, [SalesPromotionItem.Create(2, 5, DiscountType.FixedPrice, 8)]);
        var promotionItem = promotion.Items.Single();

        repository.GetApplicableItemAsync(item.ProductId, item.Quantity, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        item.DiscountAmountTotal.Should().Be(4);
        item.Total.Should().Be(16);
    }

    [Fact]
    public async Task ApplyAsync_WithFixedPriceHigherThanUnitPrice_AppliesAddition()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var promotion = SalesPromotion.Create("Promo", null, 1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, true, [SalesPromotionItem.Create(2, 5, DiscountType.FixedPrice, 12)]);
        var promotionItem = promotion.Items.Single();

        repository.GetApplicableItemAsync(item.ProductId, item.Quantity, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        item.AdditionalAmountTotal.Should().Be(4);
        item.Total.Should().Be(24);
    }

    [Fact]
    public async Task ApplyAsync_WhenCalledTwice_DoesNotDuplicatePromotionalAdjustments()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 4, 10);
        item.ApplyDiscount(SaleItemAdjustmentType.Manual, 1, Guid.NewGuid(), "Manager", "Manual");
        var promotion = SalesPromotion.Create("Promo", null, 1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, true, [SalesPromotionItem.Create(4, 9, DiscountType.Percentage, 10)]);
        var promotionItem = promotion.Items.Single();

        repository.GetApplicableItemAsync(item.ProductId, item.Quantity, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);
        await service.ApplyAsync(sale, CancellationToken.None);

        item.Discounts.Should().HaveCount(2);
        item.Discounts.Count(x => x.AdjustmentType == SaleItemAdjustmentType.Promotional).Should().Be(1);
        item.DiscountAmountTotal.Should().Be(5);
    }

    [Fact]
    public void Clear_RemovesOnlyPromotionalAdjustments()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        item.ApplyDiscount(SaleItemAdjustmentType.Manual, 2, Guid.NewGuid(), "Manager", "Manual");
        item.ApplyDiscount(SaleItemAdjustmentType.Promotional, 3);
        item.ApplyAddition(SaleItemAdjustmentType.Promotional, 1);
        sale.RecalculateTotals();

        service.Clear(sale);

        item.Discounts.Should().ContainSingle(x => x.AdjustmentType == SaleItemAdjustmentType.Manual);
        item.Additions.Should().BeEmpty();
        item.DiscountAmountTotal.Should().Be(2);
    }
}
