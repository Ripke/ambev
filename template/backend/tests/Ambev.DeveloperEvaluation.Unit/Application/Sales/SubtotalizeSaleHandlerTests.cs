using Ambev.DeveloperEvaluation.Application.Sales.SubtotalizeSale;
using Ambev.DeveloperEvaluation.Application.Sales.Promotions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SubtotalizeSaleHandlerTests
{
    [Fact]
    public async Task Handle_WithValidVersion_SubtotalizesSale()
    {
        var repository = Substitute.For<ISaleRepository>();
        var promotionalSaleService = Substitute.For<IPromotionalSaleService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new SubtotalizeSaleHandler(repository, promotionalSaleService, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var result = new SubtotalizeSaleResult();
        var command = new SubtotalizeSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<SubtotalizeSaleResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Status.Should().Be(Ambev.DeveloperEvaluation.Domain.Enums.SaleStatus.Subtotalized);
        response.Should().BeSameAs(result);
        await promotionalSaleService.Received(1).ApplyAsync(sale, Arg.Any<CancellationToken>());
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOutdatedVersion_ThrowsValidationException()
    {
        var repository = Substitute.For<ISaleRepository>();
        var promotionalSaleService = Substitute.For<IPromotionalSaleService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new SubtotalizeSaleHandler(repository, promotionalSaleService, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var command = new SubtotalizeSaleCommand { Id = sale.Id, Version = Guid.NewGuid() };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WithoutItems_ThrowsValidationException()
    {
        var repository = Substitute.For<ISaleRepository>();
        var promotionalSaleService = Substitute.For<IPromotionalSaleService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new SubtotalizeSaleHandler(repository, promotionalSaleService, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var command = new SubtotalizeSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WhenPromotionServiceAppliesAdjustments_DoesNotDuplicatePromotionalDiscounts()
    {
        var repository = Substitute.For<ISaleRepository>();
        var promotionalSaleService = Substitute.For<IPromotionalSaleService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new SubtotalizeSaleHandler(repository, promotionalSaleService, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 4, 10);
        item.ApplyDiscount(Ambev.DeveloperEvaluation.Domain.Enums.AdditionDiscountTypes.Promocional, 4);
        var result = new SubtotalizeSaleResult();
        var command = new SubtotalizeSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<SubtotalizeSaleResult>(sale).Returns(result);
        promotionalSaleService
            .When(x => x.ApplyAsync(sale, Arg.Any<CancellationToken>()))
            .Do(_ =>
            {
                sale.ClearPromotionalAdjustments();
                item.ApplyDiscount(Ambev.DeveloperEvaluation.Domain.Enums.AdditionDiscountTypes.Promocional, 4);
                sale.RecalculateTotals();
            });

        await handler.Handle(command, CancellationToken.None);

        item.Discounts.Should().ContainSingle(x => x.TipoDesconto == Ambev.DeveloperEvaluation.Domain.Enums.AdditionDiscountTypes.Promocional);
        item.DiscountAmountTotal.Should().Be(4);
    }
}
