using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.Promotions;

public class PromotionalSaleService : IPromotionalSaleService
{
    private readonly ISalesPromotionRepository _salesPromotionRepository;

    public PromotionalSaleService(ISalesPromotionRepository salesPromotionRepository)
    {
        _salesPromotionRepository = salesPromotionRepository;
    }

    public async Task ApplyAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        Clear(sale);

        var referenceDate = DateTime.UtcNow;
        foreach (var item in sale.Items.Where(item => !item.IsCanceled))
        {
            var promotionItem = await _salesPromotionRepository.GetApplicableItemAsync(
                item.ProductId,
                item.Quantity,
                referenceDate,
                cancellationToken);

            if (promotionItem == null)
                continue;

            var discountAmount = CalculateDiscountAmount(item, promotionItem);
            if (discountAmount > 0)
            {
                item.ApplyDiscount(AdditionDiscountTypes.Promocional, discountAmount);
                continue;
            }

            var additionAmount = CalculateAdditionAmount(item, promotionItem);
            if (additionAmount > 0)
                item.ApplyAddition(AdditionDiscountTypes.Promocional, additionAmount);
        }

        sale.RecalculateTotals();
    }

    public void Clear(Sale sale)
    {
        sale.ClearPromotionalAdjustments();
    }

    private static decimal CalculateDiscountAmount(SaleItem item, SalesPromotionItem promotionItem)
    {
        var amount = promotionItem.DiscountType switch
        {
            DiscountType.Percentage => item.Subtotal * (promotionItem.DiscountValue / 100m),
            DiscountType.FixedAmount => item.Quantity * promotionItem.DiscountValue,
            DiscountType.FixedPrice when promotionItem.DiscountValue < item.UnitPrice => item.Quantity * (item.UnitPrice - promotionItem.DiscountValue),
            _ => 0m
        };

        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal CalculateAdditionAmount(SaleItem item, SalesPromotionItem promotionItem)
    {
        if (promotionItem.DiscountType != DiscountType.FixedPrice || promotionItem.DiscountValue <= item.UnitPrice)
            return 0m;

        var amount = item.Quantity * (promotionItem.DiscountValue - item.UnitPrice);
        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
}
