using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleDomainTests
{
    [Fact]
    public void Create_ShouldInitializeOpenSale()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");

        sale.Status.Should().Be(SaleStatus.Open);
        sale.Version.Should().NotBe(Guid.Empty);
        sale.Subtotal.Should().Be(0);
        sale.Total.Should().Be(0);
        sale.Events.Should().ContainSingle(x => x is SaleCreatedEvent);
    }

    [Fact]
    public void Subtotalize_ShouldChangeStatusAndRenewVersion()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var previousVersion = sale.Version;

        sale.DequeueEvents();
        sale.Subtotalize();

        sale.Status.Should().Be(SaleStatus.Subtotalized);
        sale.Version.Should().NotBe(previousVersion);
        sale.Events.Should().ContainSingle(x => x is SaleSubtotalizedEvent);
    }

    [Fact]
    public void Reopen_ShouldChangeStatusBackToOpen()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.Subtotalize();

        sale.DequeueEvents();
        sale.Reopen();

        sale.Status.Should().Be(SaleStatus.Open);
        sale.Events.Should().ContainSingle(x => x is SaleReopenedEvent);
    }

    [Fact]
    public void Cancel_ShouldSetCancellationData()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var authorizerId = Guid.NewGuid();

        sale.DequeueEvents();
        sale.Cancel(authorizerId, "Manager User", "Erro");

        sale.Status.Should().Be(SaleStatus.Canceled);
        sale.IsCanceled.Should().BeTrue();
        sale.CancellationAuthorizerId.Should().Be(authorizerId);
        sale.CancellationAuthorizerName.Should().Be("Manager User");
        sale.CancellationReason.Should().Be("Erro");
        sale.Events.Should().ContainSingle(x => x is SaleCanceledEvent);
    }

    [Fact]
    public void Reopen_WhenStatusIsNotSubtotalized_ShouldThrow()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");

        var act = () => sale.Reopen();

        act.Should().Throw<InvalidOperationException>();
    }
}
