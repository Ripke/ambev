using Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ServiceAcrescimoDescontoTests
{
    [Fact]
    public async Task Aplicar_ManualDiscountWithManagerAuthorizer_UpdatesTotals()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var service = CreateService(saleRepository, userRepository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var authorizer = new User { Id = Guid.NewGuid(), Username = "Manager User", Role = UserRole.Manager };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        userRepository.GetByIdAsync(authorizer.Id, Arg.Any<CancellationToken>()).Returns(authorizer);

        var response = await service.Apply(
            global::Ambev.DeveloperEvaluation.Domain.Enums.SaleItemAdjustmentKind.Discount,
            SaleItemAdjustmentType.Manual,
            sale.Id,
            item.Id,
            3,
            authorizer.Id,
            "Ajuste",
            CancellationToken.None);

        response.Should().BeSameAs(sale);
        item.Discounts.Should().ContainSingle();
        item.Discounts[0].AuthorizerName.Should().Be("Manager User");
        item.Total.Should().Be(17);
        sale.Total.Should().Be(17);
        await saleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Aplicar_ManualAdditionWithAdminAuthorizer_UpdatesTotals()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var service = CreateService(saleRepository, userRepository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var authorizer = new User { Id = Guid.NewGuid(), Username = "Admin User", Role = UserRole.Admin };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        userRepository.GetByIdAsync(authorizer.Id, Arg.Any<CancellationToken>()).Returns(authorizer);

        await service.Apply(
            global::Ambev.DeveloperEvaluation.Domain.Enums.SaleItemAdjustmentKind.Addition,
            SaleItemAdjustmentType.Manual,
            sale.Id,
            item.Id,
            4,
            authorizer.Id,
            "Freight manual",
            CancellationToken.None);

        item.Additions.Should().ContainSingle();
        item.Additions[0].AuthorizerName.Should().Be("Admin User");
        item.Total.Should().Be(24);
        sale.Total.Should().Be(24);
    }

    [Fact]
    public async Task Aplicar_ManualWithoutAuthorizer_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var service = CreateService(saleRepository, userRepository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => service.Apply(
            global::Ambev.DeveloperEvaluation.Domain.Enums.SaleItemAdjustmentKind.Discount,
            SaleItemAdjustmentType.Manual,
            sale.Id,
            item.Id,
            3,
            null,
            "Ajuste",
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Aplicar_WithInvalidAuthorizerRole_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var service = CreateService(saleRepository, userRepository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        var authorizer = new User { Id = Guid.NewGuid(), Username = "Customer User", Role = UserRole.Customer };

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        userRepository.GetByIdAsync(authorizer.Id, Arg.Any<CancellationToken>()).Returns(authorizer);

        var act = () => service.Apply(
            global::Ambev.DeveloperEvaluation.Domain.Enums.SaleItemAdjustmentKind.Discount,
            SaleItemAdjustmentType.Manual,
            sale.Id,
            item.Id,
            3,
            authorizer.Id,
            "Ajuste",
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Aplicar_WhenSaleDoesNotExist_ThrowsKeyNotFoundException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var service = CreateService(saleRepository, userRepository);

        saleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => service.Apply(
            global::Ambev.DeveloperEvaluation.Domain.Enums.SaleItemAdjustmentKind.Discount,
            SaleItemAdjustmentType.Manual,
            Guid.NewGuid(),
            Guid.NewGuid(),
            3,
            Guid.NewGuid(),
            "Ajuste",
            CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Aplicar_WhenSaleIsNotOpen_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var service = CreateService(saleRepository, userRepository);
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.Subtotalize();

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => service.Apply(
            global::Ambev.DeveloperEvaluation.Domain.Enums.SaleItemAdjustmentKind.Discount,
            SaleItemAdjustmentType.Manual,
            sale.Id,
            item.Id,
            3,
            Guid.NewGuid(),
            "Ajuste",
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Aplicar_WhenStrategyIsNotRegistered_ThrowsValidationException()
    {
        var saleRepository = Substitute.For<ISaleRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var service = new ServiceAdditionDiscount(saleRepository, userRepository, [new DiscountPromotionalStrategy()]);

        var act = () => service.Apply(
            global::Ambev.DeveloperEvaluation.Domain.Enums.SaleItemAdjustmentKind.Addition,
            SaleItemAdjustmentType.Manual,
            Guid.NewGuid(),
            Guid.NewGuid(),
            3,
            Guid.NewGuid(),
            "Ajuste",
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    private static ServiceAdditionDiscount CreateService(ISaleRepository saleRepository, IUserRepository userRepository)
    {
        return new ServiceAdditionDiscount(
            saleRepository,
            userRepository,
            [
                new AdditionManualStrategy(),
                new DiscountManualStrategy(),
                new DiscountPromotionalStrategy()
            ]);
    }
}
