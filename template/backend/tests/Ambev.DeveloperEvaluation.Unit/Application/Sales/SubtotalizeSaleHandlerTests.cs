using Ambev.DeveloperEvaluation.Application.Sales.SubtotalizeSale;
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
        var mapper = Substitute.For<IMapper>();
        var handler = new SubtotalizeSaleHandler(repository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var result = new SubtotalizeSaleResult();
        var command = new SubtotalizeSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        mapper.Map<SubtotalizeSaleResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Status.Should().Be(Ambev.DeveloperEvaluation.Domain.Enums.SaleStatus.Subtotalized);
        response.Should().BeSameAs(result);
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOutdatedVersion_ThrowsValidationException()
    {
        var repository = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new SubtotalizeSaleHandler(repository, mapper);
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
        var mapper = Substitute.For<IMapper>();
        var handler = new SubtotalizeSaleHandler(repository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var command = new SubtotalizeSaleCommand { Id = sale.Id, Version = sale.Version };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
