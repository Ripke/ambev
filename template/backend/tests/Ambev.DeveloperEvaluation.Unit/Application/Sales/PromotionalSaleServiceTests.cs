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
    public async Task ApplyAsync_WithSameProductSplitAcrossItems_UsesGroupedQuantityForPercentagePromotion()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var productId = Guid.NewGuid();
        var firstItem = sale.AddItem("789", productId, "Produto", 2, 10);
        var secondItem = sale.AddItem("789", productId, "Produto", 2, 10);
        var promotionItem = CreatePromotionItem(4, 9, DiscountType.Percentage, 10);

        repository.GetApplicableItemAsync(productId, 4, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        firstItem.DiscountAmountTotal.Should().Be(2);
        secondItem.DiscountAmountTotal.Should().Be(2);
        sale.DiscountAmountTotal.Should().Be(4);
        sale.Total.Should().Be(36);
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
    public async Task ApplyAsync_WithSameProductSplitAcrossItems_UsesGroupedQuantityForFixedAmountPromotion()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var productId = Guid.NewGuid();
        var firstItem = sale.AddItem("789", productId, "Produto", 4, 10);
        var secondItem = sale.AddItem("789", productId, "Produto", 6, 10);
        var promotionItem = CreatePromotionItem(10, 20, DiscountType.FixedAmount, 2);

        repository.GetApplicableItemAsync(productId, 10, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        firstItem.DiscountAmountTotal.Should().Be(8);
        secondItem.DiscountAmountTotal.Should().Be(12);
        sale.DiscountAmountTotal.Should().Be(20);
        sale.Total.Should().Be(80);
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
    public async Task ApplyAsync_WithSameProductSplitAcrossItems_UsesGroupedQuantityForFixedPricePromotion()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var productId = Guid.NewGuid();
        var firstItem = sale.AddItem("789", productId, "Produto", 1, 10);
        var secondItem = sale.AddItem("789", productId, "Produto", 1, 12);
        var promotionItem = CreatePromotionItem(2, 5, DiscountType.FixedPrice, 8);

        repository.GetApplicableItemAsync(productId, 2, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        firstItem.DiscountAmountTotal.Should().Be(2);
        secondItem.DiscountAmountTotal.Should().Be(4);
        sale.DiscountAmountTotal.Should().Be(6);
        sale.Total.Should().Be(16);
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
    public async Task ApplyAsync_WithDifferentProducts_DoesNotMixQuantitiesAcrossGroups()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var firstProductId = Guid.NewGuid();
        var secondProductId = Guid.NewGuid();
        var firstProductItem = sale.AddItem("789", firstProductId, "Produto A", 2, 10);
        var secondProductItem = sale.AddItem("790", secondProductId, "Produto B", 2, 10);

        repository.GetApplicableItemAsync(firstProductId, 2, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((SalesPromotionItem?)null);
        repository.GetApplicableItemAsync(secondProductId, 2, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((SalesPromotionItem?)null);

        await service.ApplyAsync(sale, CancellationToken.None);

        firstProductItem.Discounts.Should().BeEmpty();
        secondProductItem.Discounts.Should().BeEmpty();
        await repository.Received(1).GetApplicableItemAsync(firstProductId, 2, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await repository.Received(1).GetApplicableItemAsync(secondProductId, 2, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplyAsync_IgnoresCanceledItemsInGroupedQuantityAndPromotionApplication()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var productId = Guid.NewGuid();
        var activeItem = sale.AddItem("789", productId, "Produto", 3, 10);
        var canceledItem = sale.AddItem("789", productId, "Produto", 1, 10);
        canceledItem.Cancel(Guid.NewGuid(), "Manager", "Canceled");
        var promotionItem = CreatePromotionItem(3, 9, DiscountType.Percentage, 10);

        repository.GetApplicableItemAsync(productId, 3, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(promotionItem);

        await service.ApplyAsync(sale, CancellationToken.None);

        activeItem.DiscountAmountTotal.Should().Be(3);
        canceledItem.Discounts.Should().BeEmpty();
        canceledItem.Additions.Should().BeEmpty();
        await repository.Received(1).GetApplicableItemAsync(productId, 3, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await repository.DidNotReceive().GetApplicableItemAsync(productId, 4, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplyAsync_DoesNotApplyPromotion_WhenThresholdIsReachedOnlyByCanceledItems()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var service = new PromotionalSaleService(repository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var productId = Guid.NewGuid();
        var activeItem = sale.AddItem("789", productId, "Produto", 3, 10);
        var canceledItem = sale.AddItem("789", productId, "Produto", 1, 10);
        canceledItem.Cancel(Guid.NewGuid(), "Manager", "Canceled");

        repository.GetApplicableItemAsync(productId, 3, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((SalesPromotionItem?)null);

        await service.ApplyAsync(sale, CancellationToken.None);

        activeItem.Discounts.Should().BeEmpty();
        canceledItem.Discounts.Should().BeEmpty();
        sale.DiscountAmountTotal.Should().Be(0);
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

    private static SalesPromotionItem CreatePromotionItem(int minimumQuantity, int maximumQuantity, DiscountType discountType, decimal discountValue)
    {
        return SalesPromotionItem.Create(minimumQuantity, maximumQuantity, discountType, discountValue);
    }
}
