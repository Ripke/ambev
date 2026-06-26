using Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItemQuantity;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SaleItemHandlersTests
{
    [Fact]
    public async Task AddSaleItem_ByProductId_AddsItem()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var priceRepository = Substitute.For<IProductPriceRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new AddSaleItemHandler(saleRepository, productRepository, priceRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var product = Product.Create("Produto", "UN", "Marca", "Modelo", ProductType.Normal, 10, ["789"]);
        var price = ProductPrice.Create(product.Id, PriceType.Cash, 12, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        var result = new AddSaleItemResult();
        var command = new AddSaleItemCommand { SaleId = sale.Id, ProductId = product.Id, Quantity = 2 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        productRepository.GetByIdWithDetailsAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        priceRepository.GetCurrentPriceByProductIdAsync(product.Id, PriceType.Cash, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(price);
        mapper.Map<AddSaleItemResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Items.Should().ContainSingle();
        sale.Items[0].ProductId.Should().Be(product.Id);
        sale.Total.Should().Be(24);
        response.Should().BeSameAs(result);
    }

    [Fact]
    public async Task AddSaleItem_ByEan_AddsItem()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var priceRepository = Substitute.For<IProductPriceRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new AddSaleItemHandler(saleRepository, productRepository, priceRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var product = Product.Create("Produto", "UN", "Marca", "Modelo", ProductType.Normal, 10, ["789"]);
        var price = ProductPrice.Create(product.Id, PriceType.Cash, 12, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        var result = new AddSaleItemResult();
        var command = new AddSaleItemCommand { SaleId = sale.Id, Ean = "789", Quantity = 2 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        productRepository.GetByBarcodeAsync("789", Arg.Any<CancellationToken>()).Returns(product);
        priceRepository.GetCurrentPriceByProductIdAsync(product.Id, PriceType.Cash, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(price);
        mapper.Map<AddSaleItemResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Items.Should().ContainSingle();
        sale.Items[0].ProductEan.Should().Be("789");
        response.Should().BeSameAs(result);
    }

    [Fact]
    public async Task AddSaleItem_WhenSaleIsNotOpen_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var priceRepository = Substitute.For<IProductPriceRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new AddSaleItemHandler(saleRepository, productRepository, priceRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.Subtotalize();
        var command = new AddSaleItemCommand { SaleId = sale.Id, ProductId = Guid.NewGuid(), Quantity = 2 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AddSaleItem_WithoutCurrentPrice_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var priceRepository = Substitute.For<IProductPriceRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new AddSaleItemHandler(saleRepository, productRepository, priceRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var product = Product.Create("Produto", "UN", "Marca", "Modelo", ProductType.Normal, 10, ["789"]);
        var command = new AddSaleItemCommand { SaleId = sale.Id, ProductId = product.Id, Quantity = 2 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        productRepository.GetByIdWithDetailsAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        priceRepository.GetCurrentPriceByProductIdAsync(product.Id, PriceType.Cash, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns((ProductPrice?)null);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AddSaleItem_WhenExceedsMaxSaleQuantity_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var priceRepository = Substitute.For<IProductPriceRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new AddSaleItemHandler(saleRepository, productRepository, priceRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var productId = Guid.NewGuid();
        sale.AddItem("789", productId, "Produto", 8, 10);
        var product = Product.Create("Produto", "UN", "Marca", "Modelo", ProductType.Normal, 10, ["789"]);
        product.Id = productId;
        var price = ProductPrice.Create(productId, PriceType.Cash, 12, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        var command = new AddSaleItemCommand { SaleId = sale.Id, ProductId = productId, Quantity = 3 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        productRepository.GetByIdWithDetailsAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        priceRepository.GetCurrentPriceByProductIdAsync(productId, PriceType.Cash, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(price);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateSaleItemQuantity_WhenItemIsCanceled_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new UpdateSaleItemQuantityHandler(saleRepository, productRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.CancelItem(item.Id, Guid.NewGuid(), "Manager", "Reason");
        var command = new UpdateSaleItemQuantityCommand { SaleId = sale.Id, ItemId = item.Id, Quantity = 5 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateSaleItemQuantity_WhenSaleIsNotOpen_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new UpdateSaleItemQuantityHandler(saleRepository, productRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.Subtotalize();
        var command = new UpdateSaleItemQuantityCommand { SaleId = sale.Id, ItemId = item.Id, Quantity = 5 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateSaleItemQuantity_RecalculatesItemAndSale()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new UpdateSaleItemQuantityHandler(saleRepository, productRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var product = Product.Create("Produto", "UN", "Marca", "Modelo", ProductType.Normal, 10, ["789"]);
        var item = sale.AddItem("789", product.Id, "Produto", 2, 10);
        var result = new UpdateSaleItemQuantityResult();
        var command = new UpdateSaleItemQuantityCommand { SaleId = sale.Id, ItemId = item.Id, Quantity = 5 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        mapper.Map<UpdateSaleItemQuantityResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        item.Quantity.Should().Be(5);
        item.Total.Should().Be(50);
        sale.Total.Should().Be(50);
        response.Should().BeSameAs(result);
    }

    [Fact]
    public async Task AddSaleItem_WhenOnlyCanceledItemsExist_DoesNotCountThemInMaxSaleQuantity()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var priceRepository = Substitute.For<IProductPriceRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new AddSaleItemHandler(saleRepository, productRepository, priceRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var productId = Guid.NewGuid();
        var canceledItem = sale.AddItem("789", productId, "Produto", 10, 10);
        sale.CancelItem(canceledItem.Id, Guid.NewGuid(), "Manager", "Reason");
        var product = Product.Create("Produto", "UN", "Marca", "Modelo", ProductType.Normal, 10, ["789"]);
        product.Id = productId;
        var price = ProductPrice.Create(productId, PriceType.Cash, 12, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        var result = new AddSaleItemResult();
        var command = new AddSaleItemCommand { SaleId = sale.Id, ProductId = productId, Quantity = 10 };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        productRepository.GetByIdWithDetailsAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        priceRepository.GetCurrentPriceByProductIdAsync(productId, PriceType.Cash, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(price);
        mapper.Map<AddSaleItemResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Items.Count.Should().Be(2);
        sale.Items.Last().Quantity.Should().Be(10);
        response.Should().BeSameAs(result);
    }

    [Fact]
    public async Task CancelSaleItem_WithManagerAuthorizer_CancelsItem()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new CancelSaleItemHandler(saleRepository, userRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var authorizer = new User { Id = Guid.NewGuid(), Username = "Manager User", Role = UserRole.Manager };
        var result = new CancelSaleItemResult();
        var command = new CancelSaleItemCommand
        {
            SaleId = sale.Id,
            ItemId = item.Id,
            CancellationAuthorizerId = authorizer.Id,
            CancellationReason = "Reason"
        };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        userRepository.GetByIdAsync(authorizer.Id, Arg.Any<CancellationToken>()).Returns(authorizer);
        mapper.Map<CancelSaleItemResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        item.IsCanceled.Should().BeTrue();
        item.Total.Should().Be(20);
        sale.Total.Should().Be(0);
        response.Should().BeSameAs(result);
    }

    [Fact]
    public async Task CancelSaleItem_WhenAlreadyCanceled_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new CancelSaleItemHandler(saleRepository, userRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.CancelItem(item.Id, Guid.NewGuid(), "Manager", "Reason");
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = item.Id, CancellationAuthorizerId = Guid.NewGuid() };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CancelSaleItem_WhenAuthorizerIsInvalid_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new CancelSaleItemHandler(saleRepository, userRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var authorizer = new User { Id = Guid.NewGuid(), Username = "Customer", Role = UserRole.Customer };
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = item.Id, CancellationAuthorizerId = authorizer.Id };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        userRepository.GetByIdAsync(authorizer.Id, Arg.Any<CancellationToken>()).Returns(authorizer);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CancelSaleItem_WhenSaleIsSubtotalized_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new CancelSaleItemHandler(saleRepository, userRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.Subtotalize();
        var command = new CancelSaleItemCommand { SaleId = sale.Id, ItemId = item.Id, CancellationAuthorizerId = Guid.NewGuid() };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
