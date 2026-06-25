using Ambev.DeveloperEvaluation.Application.Products.AddProductBarcode;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class AddProductBarcodeHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly AddProductBarcodeHandler _handler;

    public AddProductBarcodeHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new AddProductBarcodeHandler(_productRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidBarcode_AddsBarcode()
    {
        var productId = Guid.NewGuid();
        var command = new AddProductBarcodeCommand
        {
            ProductId = productId,
            Barcode = "789000000001"
        };

        _productRepository.BarcodeExistsAsync(command.Barcode, Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);
        _productRepository.GetByIdWithDetailsAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Product.Create("Rice", "KG", "Brand", "Model", ProductType.Normal, 10, []));
        _mapper.Map<AddProductBarcodeResult>(Arg.Any<ProductBarcode>()).Returns(callInfo =>
        {
            var created = callInfo.Arg<ProductBarcode>();
            return new AddProductBarcodeResult
            {
                Id = created.Id,
                ProductId = created.ProductId,
                Barcode = created.Barcode,
                CreatedAt = created.CreatedAt
            };
        });

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Barcode.Should().Be(command.Barcode);
        await _productRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateBarcode_ThrowsInvalidOperationException()
    {
        var command = new AddProductBarcodeCommand
        {
            ProductId = Guid.NewGuid(),
            Barcode = "789000000001"
        };

        _productRepository.GetByIdWithDetailsAsync(command.ProductId, Arg.Any<CancellationToken>())
            .Returns(Product.Create("Rice", "KG", "Brand", "Model", ProductType.Normal, 10, []));
        _productRepository.BarcodeExistsAsync(command.Barcode, Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
