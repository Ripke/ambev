using Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByUser;
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
    public async Task GetCurrentSaleByUser_ShouldReturnCurrentSale()
    {
        var repository = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new GetCurrentSaleByUserHandler(repository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var result = new GetCurrentSaleByUserResult();

        repository.GetCurrentByUserIdAsync(sale.UserId, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<GetCurrentSaleByUserResult>(sale).Returns(result);

        var response = await handler.Handle(new GetCurrentSaleByUserCommand(sale.UserId), CancellationToken.None);

        response.Should().BeSameAs(result);
    }
}
