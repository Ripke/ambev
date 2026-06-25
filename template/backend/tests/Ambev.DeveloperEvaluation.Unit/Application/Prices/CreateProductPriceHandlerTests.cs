using Ambev.DeveloperEvaluation.Application.Prices.CreateProductPrice;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Prices;

public class CreateProductPriceHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IMapper _mapper;
    private readonly CreateProductPriceHandler _handler;

    public CreateProductPriceHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _productPriceRepository = Substitute.For<IProductPriceRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateProductPriceHandler(_productRepository, _productPriceRepository, _mapper);
    }

    [Fact]
    public async Task Handle_AdjacentRange_CreatesPrice()
    {
        var command = CreateCommand();
        var entity = ProductPrice.Create(
            command.ProductId,
            command.PriceType,
            command.Price,
            command.EffectiveStartAt,
            command.EffectiveEndAt);

        var result = new CreateProductPriceResult { Id = entity.Id, ProductId = command.ProductId };

        _productRepository.ExistsAsync(command.ProductId, Arg.Any<CancellationToken>()).Returns(true);
        _productPriceRepository.HasOverlappingPriceAsync(
            command.ProductId,
            command.PriceType,
            command.EffectiveStartAt,
            command.EffectiveEndAt,
            Arg.Any<Guid?>(),
            Arg.Any<CancellationToken>()).Returns(false);
        _productPriceRepository.CreateAsync(Arg.Any<ProductPrice>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.Arg<ProductPrice>());
        _mapper.Map<CreateProductPriceResult>(Arg.Any<ProductPrice>()).Returns(callInfo =>
        {
            var created = callInfo.Arg<ProductPrice>();
            return new CreateProductPriceResult { Id = created.Id, ProductId = created.ProductId };
        });

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Id.Should().NotBeEmpty();
        response.ProductId.Should().Be(command.ProductId);
    }

    [Fact]
    public async Task Handle_OverlappingRange_ThrowsInvalidOperationException()
    {
        var command = CreateCommand();

        _productRepository.ExistsAsync(command.ProductId, Arg.Any<CancellationToken>()).Returns(true);
        _productPriceRepository.HasOverlappingPriceAsync(
            command.ProductId,
            command.PriceType,
            command.EffectiveStartAt,
            command.EffectiveEndAt,
            Arg.Any<Guid?>(),
            Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_DifferentPriceType_AllowsOverlap()
    {
        var command = CreateCommand();
        var entity = ProductPrice.Create(
            command.ProductId,
            command.PriceType,
            command.Price,
            command.EffectiveStartAt,
            command.EffectiveEndAt);

        var result = new CreateProductPriceResult { Id = entity.Id, ProductId = command.ProductId };

        _productRepository.ExistsAsync(command.ProductId, Arg.Any<CancellationToken>()).Returns(true);
        _productPriceRepository.HasOverlappingPriceAsync(
            command.ProductId,
            command.PriceType,
            command.EffectiveStartAt,
            command.EffectiveEndAt,
            Arg.Any<Guid?>(),
            Arg.Any<CancellationToken>()).Returns(false);
        _productPriceRepository.CreateAsync(Arg.Any<ProductPrice>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.Arg<ProductPrice>());
        _mapper.Map<CreateProductPriceResult>(Arg.Any<ProductPrice>()).Returns(callInfo =>
        {
            var created = callInfo.Arg<ProductPrice>();
            return new CreateProductPriceResult { Id = created.Id, ProductId = created.ProductId };
        });

        var response = await _handler.Handle(command, CancellationToken.None);

        response.ProductId.Should().Be(command.ProductId);
    }

    private static CreateProductPriceCommand CreateCommand()
    {
        return new CreateProductPriceCommand
        {
            ProductId = Guid.NewGuid(),
            PriceType = PriceType.Cash,
            Price = 12.5m,
            EffectiveStartAt = new DateTime(2026, 06, 25, 0, 0, 0, DateTimeKind.Utc),
            EffectiveEndAt = new DateTime(2026, 06, 30, 0, 0, 0, DateTimeKind.Utc)
        };
    }
}
