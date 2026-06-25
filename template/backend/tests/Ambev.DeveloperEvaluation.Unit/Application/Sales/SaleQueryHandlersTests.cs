using Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByCustomer;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SaleQueryHandlersTests
{
    [Fact]
    public async Task GetSale_ShouldReturnSale()
    {
        var repository = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new GetSaleHandler(repository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var result = new GetSaleResult();

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<GetSaleResult>(sale).Returns(result);

        var response = await handler.Handle(new GetSaleCommand(sale.Id), CancellationToken.None);

        response.Should().BeSameAs(result);
    }

    [Fact]
    public async Task GetCurrentSaleByCustomer_ShouldReturnCurrentSale()
    {
        var repository = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new GetCurrentSaleByCustomerHandler(repository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var result = new GetCurrentSaleByCustomerResult();

        repository.GetCurrentByCustomerIdAsync(sale.CustomerId, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<GetCurrentSaleByCustomerResult>(sale).Returns(result);

        var response = await handler.Handle(new GetCurrentSaleByCustomerCommand(sale.CustomerId), CancellationToken.None);

        response.Should().BeSameAs(result);
    }
}
