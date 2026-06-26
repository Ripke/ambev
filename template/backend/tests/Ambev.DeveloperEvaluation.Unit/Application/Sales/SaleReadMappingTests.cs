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
        item.ApplyDiscount(AdditionDiscountTypes.Manual, 2, Guid.NewGuid(), "Manager", "Desconto");
        item.ApplyAddition(AdditionDiscountTypes.Manual, 5, Guid.NewGuid(), "Manager", "Acrescimo");
        sale.RecalculateTotals();

        var result = mapper.Map<GetSaleResult>(sale);

        result.Items.Should().ContainSingle();
        result.Items[0].Discounts.Should().ContainSingle();
        result.Items[0].Additions.Should().ContainSingle();
        result.Items[0].Discounts[0].Valor.Should().Be(2);
        result.Items[0].Additions[0].Valor.Should().Be(5);
        result.Items[0].Total.Should().Be(23);
        result.Total.Should().Be(23);
    }
}
