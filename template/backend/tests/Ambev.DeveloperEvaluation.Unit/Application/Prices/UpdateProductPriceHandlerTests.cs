using Ambev.DeveloperEvaluation.Application.Prices.UpdateProductPrice;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Prices;

public class UpdateProductPriceHandlerTests
{
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IMapper _mapper;
    private readonly UpdateProductPriceHandler _handler;

    public UpdateProductPriceHandlerTests()
    {
        _productPriceRepository = Substitute.For<IProductPriceRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateProductPriceHandler(_productPriceRepository, _mapper);
    }

    [Fact]
    public async Task Handle_OverlapOnUpdate_ThrowsInvalidOperationException()
    {
        var price = ProductPrice.Create(
            Guid.NewGuid(),
            PriceType.Cash,
            10,
            new DateTime(2026, 06, 20, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 06, 24, 0, 0, 0, DateTimeKind.Utc));

        var command = new UpdateProductPriceCommand
        {
            Id = price.Id,
            PriceType = PriceType.Cash,
            Price = 11,
            EffectiveStartAt = new DateTime(2026, 06, 25, 0, 0, 0, DateTimeKind.Utc),
            EffectiveEndAt = new DateTime(2026, 06, 27, 0, 0, 0, DateTimeKind.Utc)
        };

        _productPriceRepository.GetByIdAsync(price.Id, Arg.Any<CancellationToken>()).Returns(price);
        _productPriceRepository.HasOverlappingPriceAsync(
            price.ProductId,
            command.PriceType,
            command.EffectiveStartAt,
            command.EffectiveEndAt,
            command.Id,
            Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
