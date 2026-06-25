using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleHandlerTests
{
    [Fact]
    public async Task Handle_WithManagerAuthorizer_CancelsSale()
    {
        var repository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new CancelSaleHandler(repository, userRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var result = new CancelSaleResult();
        var authorizer = new User { Id = Guid.NewGuid(), Username = "Manager User", Role = UserRole.Manager };
        var command = new CancelSaleCommand
        {
            Id = sale.Id,
            Version = sale.Version,
            CancellationAuthorizerId = authorizer.Id,
            CancellationReason = "Erro"
        };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        userRepository.GetByIdAsync(authorizer.Id, Arg.Any<CancellationToken>()).Returns(authorizer);
        mapper.Map<CancelSaleResult>(sale).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        sale.Status.Should().Be(SaleStatus.Canceled);
        sale.CancellationAuthorizerName.Should().Be("Manager User");
        response.Should().BeSameAs(result);
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidAuthorizerRole_ThrowsValidationException()
    {
        var repository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new CancelSaleHandler(repository, userRepository, mapper);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var authorizer = new User { Id = Guid.NewGuid(), Username = "Customer User", Role = UserRole.Customer };
        var command = new CancelSaleCommand
        {
            Id = sale.Id,
            Version = sale.Version,
            CancellationAuthorizerId = authorizer.Id
        };

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        userRepository.GetByIdAsync(authorizer.Id, Arg.Any<CancellationToken>()).Returns(authorizer);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
