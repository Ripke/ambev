using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SaleReadMappingTests
{
    [Fact]
    public void GetSaleMapping_ShouldIncludeDiscountsAndAdditions()
    {
        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SaleItemProfile>();
            cfg.AddProfile<GetSaleProfile>();
        }).CreateMapper();

        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        var item = sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 10);
        item.ApplyDiscount(SaleItemAdjustmentType.Manual, 2, Guid.NewGuid(), "Manager", "Discount");
        item.ApplyAddition(SaleItemAdjustmentType.Manual, 5, Guid.NewGuid(), "Manager", "Addition");
        sale.RecalculateTotals();

        var result = mapper.Map<GetSaleResult>(sale);

        result.Items.Should().ContainSingle();
        result.Items[0].Discounts.Should().ContainSingle();
        result.Items[0].Additions.Should().ContainSingle();
        result.Items[0].Discounts[0].Amount.Should().Be(2);
        result.Items[0].Additions[0].Amount.Should().Be(5);
        result.Items[0].Total.Should().Be(23);
        result.Total.Should().Be(23);
    }

    [Fact]
    public void GetSaleMapping_ShouldIncludePaymentsAndChanges()
    {
        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SaleItemProfile>();
            cfg.AddProfile<GetSaleProfile>();
        }).CreateMapper();

        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 50);
        sale.Subtotalize();
        sale.RegisterPayment(PaymentType.Cash, 120);

        var result = mapper.Map<GetSaleResult>(sale);

        result.Payments.Should().ContainSingle();
        result.Changes.Should().ContainSingle();
        result.Payments[0].Value.Should().Be(120);
        result.Changes[0].Value.Should().Be(20);
        result.ChangeAmountTotal.Should().Be(20);
        result.Status.Should().Be(SaleStatus.EmittingNfce);
    }
}
