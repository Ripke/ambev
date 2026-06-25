using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class ProductDomainEventsTests
{
    [Fact]
    public void Create_ShouldRegisterCreationAndBarcodeAssociationEvents()
    {
        var product = Product.Create("Rice", "KG", "Brand", "Model", ProductType.Normal, 10, ["789"]);

        product.Events.Should().ContainSingle(x => x is ProductCreatedEvent);
        product.Events.Should().ContainSingle(x => x is ProductBarcodeAssociatedEvent);
    }

    [Fact]
    public void Update_ShouldRegisterProductUpdatedEvent()
    {
        var product = Product.Create("Rice", "KG", "Brand", "Model", ProductType.Normal, 10, ["789"]);

        product.Update("Rice 2", "UN", "Brand 2", "Model 2", ProductType.Weighable, 20);

        product.Events.Should().ContainSingle(x => x is ProductUpdatedEvent);
    }
}
