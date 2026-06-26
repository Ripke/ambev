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
        var activeItemsByProduct = sale.Items
            .Where(item => !item.IsCanceled)
            .GroupBy(item => item.ProductId);

        foreach (var group in activeItemsByProduct)
        {
            var groupedItems = group.ToList();
            var totalQuantity = groupedItems.Sum(item => item.Quantity);
            var promotionItem = await _salesPromotionRepository.GetApplicableItemAsync(
                group.Key,
                totalQuantity,
                referenceDate,
                cancellationToken);

            if (promotionItem == null)
                continue;

            foreach (var item in groupedItems)
            {
                var discountAmount = CalculateDiscountAmount(item, promotionItem);
                if (discountAmount > 0)
                {
                    item.ApplyDiscount(SaleItemAdjustmentType.Promotional, discountAmount);
                    continue;
                }

                var additionAmount = CalculateAdditionAmount(item, promotionItem);
                if (additionAmount > 0)
                    item.ApplyAddition(SaleItemAdjustmentType.Promotional, additionAmount);
            }
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
