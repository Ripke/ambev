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
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
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
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
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

    [Fact]
    public void AddItem_ShouldRecalculateSaleTotals()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");

        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 3, 5);

        item.Subtotal.Should().Be(15);
        item.Total.Should().Be(15);
        item.SaleDateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        sale.Subtotal.Should().Be(15);
        sale.Total.Should().Be(15);
    }

    [Fact]
    public void CancelItem_ShouldKeepItemValuesAndExcludeFromSaleTotals()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 3, 5);

        sale.CancelItem(item.Id, Guid.NewGuid(), "Manager", "Reason");

        item.IsCanceled.Should().BeTrue();
        item.Subtotal.Should().Be(15);
        item.Total.Should().Be(15);
        sale.Subtotal.Should().Be(0);
        sale.Total.Should().Be(0);
    }

    [Fact]
    public void CancelItem_Twice_ShouldThrow()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 3, 5);
        sale.CancelItem(item.Id, Guid.NewGuid(), "Manager", "Reason");

        var act = () => sale.CancelItem(item.Id, Guid.NewGuid(), "Manager", "Reason");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ApplyManualDiscount_ShouldRecalculateItemAndSaleTotals()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);

        item.ApplyDiscount(SaleItemAdjustmentType.Manual, 3, Guid.NewGuid(), "Manager", "Ajuste");
        sale.RecalculateTotals();

        item.DiscountAmountTotal.Should().Be(3);
        item.Total.Should().Be(17);
        sale.DiscountAmountTotal.Should().Be(3);
        sale.Total.Should().Be(17);
    }

    [Fact]
    public void ApplyManualAddition_ShouldRecalculateItemAndSaleTotals()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);

        item.ApplyAddition(SaleItemAdjustmentType.Manual, 4, Guid.NewGuid(), "Manager", "Ajuste");
        sale.RecalculateTotals();

        item.AdditionalAmountTotal.Should().Be(4);
        item.Total.Should().Be(24);
        sale.AdditionalAmountTotal.Should().Be(4);
        sale.Total.Should().Be(24);
    }

    [Fact]
    public void ApplyAdjustments_ShouldAccumulateTotals()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);

        item.ApplyDiscount(SaleItemAdjustmentType.Manual, 2, Guid.NewGuid(), "Manager", "Ajuste");
        item.ApplyDiscount(SaleItemAdjustmentType.Promotional, 1, null, null, "Promo");
        item.ApplyAddition(SaleItemAdjustmentType.Manual, 5, Guid.NewGuid(), "Manager", "Ajuste");
        sale.RecalculateTotals();

        item.Discounts.Should().HaveCount(2);
        item.Additions.Should().HaveCount(1);
        item.DiscountAmountTotal.Should().Be(3);
        item.AdditionalAmountTotal.Should().Be(5);
        item.Total.Should().Be(22);
        sale.Total.Should().Be(22);
    }

    [Fact]
    public void ApplyAdjustment_WithInvalidValue_ShouldThrow()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);

        var act = () => item.ApplyDiscount(SaleItemAdjustmentType.Manual, 0, Guid.NewGuid(), "Manager", "Ajuste");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Subtotalize_WithoutActiveItems_ShouldThrow()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");

        var act = () => sale.Subtotalize();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RegisterPayment_FirstPartialPayment_ShouldMoveSaleToPaymentCompleted()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        sale.Subtotalize();

        sale.RegisterPayment(PaymentType.Cash, 5);

        sale.Status.Should().Be(SaleStatus.PaymentCompleted);
        sale.PaymentAmountTotal.Should().Be(5);
        sale.ChangeAmountTotal.Should().Be(0);
        sale.Payments.Should().ContainSingle();
        sale.Version.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void RegisterPayment_FinalCashPaymentWithChange_ShouldGenerateChangeAndEmitNfce()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 50);
        sale.Subtotalize();

        sale.RegisterPayment(PaymentType.Cash, 20);
        sale.RegisterPayment(PaymentType.CreditCard, 5);
        sale.RegisterPayment(PaymentType.DebitCard, 12);
        sale.RegisterPayment(PaymentType.Cash, 80);

        sale.Status.Should().Be(SaleStatus.EmittingNfce);
        sale.PaymentAmountTotal.Should().Be(117);
        sale.ChangeAmountTotal.Should().Be(17);
        sale.Payments.Should().HaveCount(4);
        sale.Changes.Should().ContainSingle();
        sale.Changes[0].Value.Should().Be(17);
    }

    [Fact]
    public void RegisterPayment_NonCashOverpayment_ShouldThrow()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 50);
        sale.Subtotalize();
        sale.RegisterPayment(PaymentType.Cash, 95);

        var act = () => sale.RegisterPayment(PaymentType.CreditCard, 10);

        act.Should().Throw<InvalidOperationException>();
    }
}
