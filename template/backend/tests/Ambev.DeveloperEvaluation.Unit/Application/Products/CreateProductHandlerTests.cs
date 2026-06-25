using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateProductHandler(_productRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequestWithSingleBarcode_ReturnsCreatedProduct()
    {
        var command = CreateValidCommand(["789000000001"]);
        var product = CreateMappedProduct(command);
        var result = new CreateProductResult { Id = Guid.NewGuid(), Description = command.Description };

        _mapper.Map<CreateProductResult>(Arg.Any<Product>()).Returns(result);
        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.BarcodeExistsAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        await _productRepository.Received(1).CreateAsync(
            Arg.Is<Product>(x => x.Barcodes.Count == 1 && x.Barcodes[0].Barcode == "789000000001"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidRequestWithMultipleBarcodes_CreatesAllBarcodes()
    {
        var command = CreateValidCommand(["789000000001", "789000000002"]);
        var product = CreateMappedProduct(command);
        var result = new CreateProductResult { Id = Guid.NewGuid() };

        _mapper.Map<CreateProductResult>(Arg.Any<Product>()).Returns(result);
        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.BarcodeExistsAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        await _handler.Handle(command, CancellationToken.None);

        await _productRepository.Received(1).CreateAsync(
            Arg.Is<Product>(x =>
                x.Barcodes.Count == 2 &&
                x.Barcodes.Any(b => b.Barcode == "789000000001") &&
                x.Barcodes.Any(b => b.Barcode == "789000000002")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateBarcodesInPayload_ThrowsValidationException()
    {
        var command = CreateValidCommand(["789000000001", "789000000001"]);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task Handle_BarcodeAlreadyExists_ThrowsInvalidOperationException()
    {
        var command = CreateValidCommand(["789000000001"]);

        _productRepository.BarcodeExistsAsync("789000000001", Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private static CreateProductCommand CreateValidCommand(List<string> barcodes)
    {
        return new CreateProductCommand
        {
            Description = "Rice",
            UnitMeasure = "KG",
            Brand = "Brand",
            Model = "Model",
            ProductType = ProductType.Normal,
            MaxSaleQuantity = 10,
            Barcodes = barcodes
        };
    }

    private static Product CreateMappedProduct(CreateProductCommand command)
    {
        return Product.Create(
            command.Description,
            command.UnitMeasure,
            command.Brand,
            command.Model,
            command.ProductType,
            command.MaxSaleQuantity,
            command.Barcodes);
    }
}
