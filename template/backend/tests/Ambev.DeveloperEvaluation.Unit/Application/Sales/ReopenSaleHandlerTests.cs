using Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
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
        var mapper = Substitute.For<IMapper>();
        var handler = new ReopenSaleHandler(repository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.Subtotalize();
        var result = new ReopenSaleResult();
        var command = new ReopenSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<ReopenSaleResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Status.Should().Be(Ambev.DeveloperEvaluation.Domain.Enums.SaleStatus.Open);
        response.Should().BeSameAs(result);
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
