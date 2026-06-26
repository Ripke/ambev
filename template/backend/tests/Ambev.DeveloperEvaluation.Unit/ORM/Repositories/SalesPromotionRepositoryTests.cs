using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.ORM.Repositories;

public class SalesPromotionRepositoryTests
{
    [Fact]
    public async Task GetApplicableItemAsync_SelectsHigherPriorityPromotion()
    {
        await using var context = CreateContext();
        var repository = new SalesPromotionRepository(context);
        var productId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        context.SalesPromotions.Add(CreatePromotion("Low", 1, now, productId, 4, 9, 10));
        context.SalesPromotions.Add(CreatePromotion("High", 2, now, productId, 4, 9, 20));
        await context.SaveChangesAsync();

        var result = await repository.GetApplicableItemAsync(productId, 4, now, CancellationToken.None);

        result.Should().NotBeNull();
        result!.DiscountValue.Should().Be(20);
    }

    [Fact]
    public async Task GetApplicableItemAsync_WhenPriorityTies_SelectsOldestPromotion()
    {
        await using var context = CreateContext();
        var repository = new SalesPromotionRepository(context);
        var productId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var older = CreatePromotion("Older", 1, now, productId, 4, 9, 10);
        var newer = CreatePromotion("Newer", 1, now, productId, 4, 9, 20);
        typeof(SalesPromotion).GetProperty(nameof(SalesPromotion.CreatedAt))!.SetValue(older, now.AddMinutes(-10));
        typeof(SalesPromotion).GetProperty(nameof(SalesPromotion.CreatedAt))!.SetValue(newer, now);

        context.SalesPromotions.AddRange(older, newer);
        await context.SaveChangesAsync();

        var result = await repository.GetApplicableItemAsync(productId, 4, now, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Promotion.Name.Should().Be("Older");
    }

    [Fact]
    public async Task GetApplicableItemAsync_IgnoresInactiveAndOutOfRangePromotions()
    {
        await using var context = CreateContext();
        var repository = new SalesPromotionRepository(context);
        var productId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        context.SalesPromotions.Add(CreatePromotion("Inactive", 2, now, productId, 4, 9, 20, isActive: false));
        context.SalesPromotions.Add(CreatePromotion("Expired", 3, now.AddDays(-5), productId, 4, 9, 30, endDate: now.AddDays(-1)));
        context.SalesPromotions.Add(CreatePromotion("Active", 1, now, productId, 4, 9, 10));
        await context.SaveChangesAsync();

        var result = await repository.GetApplicableItemAsync(productId, 4, now, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Promotion.Name.Should().Be("Active");
    }

    [Fact]
    public async Task GetApplicableItemAsync_AllowsGlobalPromotionWhenNoSpecificIsHigherByOrdering()
    {
        await using var context = CreateContext();
        var repository = new SalesPromotionRepository(context);
        var productId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        context.SalesPromotions.Add(CreatePromotion("Global", 2, now, null, 4, 9, 10));
        context.SalesPromotions.Add(CreatePromotion("Specific", 1, now, productId, 4, 9, 20));
        await context.SaveChangesAsync();

        var result = await repository.GetApplicableItemAsync(productId, 4, now, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Promotion.Name.Should().Be("Global");
    }

    private static DefaultContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DefaultContext(options);
    }

    private static SalesPromotion CreatePromotion(
        string name,
        int priority,
        DateTime startDate,
        Guid? productId,
        int minimumQuantity,
        int maximumQuantity,
        decimal discountValue,
        bool isActive = true,
        DateTime? endDate = null)
    {
        return SalesPromotion.Create(
            name,
            null,
            priority,
            startDate.AddDays(-1),
            endDate ?? startDate.AddDays(1),
            productId,
            isActive,
            [SalesPromotionItem.Create(minimumQuantity, maximumQuantity, DiscountType.Percentage, discountValue)]);
    }
}
