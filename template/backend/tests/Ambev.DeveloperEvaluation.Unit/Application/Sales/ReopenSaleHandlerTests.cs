using Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;
using Ambev.DeveloperEvaluation.Application.Sales.Promotions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ReopenSaleHandlerTests
{
    [Fact]
    public async Task Handle_WithValidVersion_ReopensSale()
    {
        var repository = Substitute.For<ISaleRepository>();
        var promotionalSaleService = Substitute.For<IPromotionalSaleService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new ReopenSaleHandler(repository, promotionalSaleService, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.Subtotalize();
        var result = new ReopenSaleResult();
        var command = new ReopenSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<ReopenSaleResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Status.Should().Be(Ambev.DeveloperEvaluation.Domain.Enums.SaleStatus.Open);
        response.Should().BeSameAs(result);
        promotionalSaleService.Received(1).Clear(sale);
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ClearsOnlyPromotionalAdjustments()
    {
        var repository = Substitute.For<ISaleRepository>();
        var promotionalSaleService = Substitute.For<IPromotionalSaleService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new ReopenSaleHandler(repository, promotionalSaleService, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        item.ApplyDiscount(SaleItemAdjustmentType.Manual, 2, Guid.NewGuid(), "Manager", "Manual");
        item.ApplyDiscount(SaleItemAdjustmentType.Promotional, 3);
        item.ApplyAddition(SaleItemAdjustmentType.Promotional, 1);
        sale.RecalculateTotals();
        sale.Subtotalize();
        var result = new ReopenSaleResult();
        var command = new ReopenSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<ReopenSaleResult>(sale).Returns(result);
        promotionalSaleService.When(x => x.Clear(sale)).Do(_ => sale.ClearPromotionalAdjustments());

        await handler.Handle(command, CancellationToken.None);

        item.Discounts.Should().ContainSingle(x => x.AdjustmentType == SaleItemAdjustmentType.Manual);
        item.Additions.Should().BeEmpty();
        item.DiscountAmountTotal.Should().Be(2);
    }
}
