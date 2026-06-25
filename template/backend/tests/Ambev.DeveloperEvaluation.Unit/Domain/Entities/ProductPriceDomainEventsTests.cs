using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class ProductPriceDomainEventsTests
{
    [Fact]
    public void Create_ShouldRegisterCreatedEvent()
    {
        var price = ProductPrice.Create(
            Guid.NewGuid(),
            PriceType.Cash,
            12.5m,
            new DateTime(2026, 06, 25, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 06, 30, 0, 0, 0, DateTimeKind.Utc));

        price.Events.Should().ContainSingle(x => x is ProductPriceCreatedEvent);
    }

    [Fact]
    public void Update_ShouldRegisterUpdatedEvent()
    {
        var price = ProductPrice.Create(
            Guid.NewGuid(),
            PriceType.Cash,
            12.5m,
            new DateTime(2026, 06, 25, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 06, 30, 0, 0, 0, DateTimeKind.Utc));

        price.Update(
            PriceType.Wholesale,
            10.5m,
            new DateTime(2026, 07, 01, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 07, 05, 0, 0, 0, DateTimeKind.Utc));

        price.Events.Should().ContainSingle(x => x is ProductPriceUpdatedEvent);
    }
}
