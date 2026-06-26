using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AcrescimoDesconto;

public class DiscountPromotionalStrategy : IAdditionDiscountStrategy
{
    public Domain.Enums.AdditionDiscount AdditionDiscount => Domain.Enums.AdditionDiscount.Desconto;
    public AdditionDiscountTypes AdditionDiscountType => AdditionDiscountTypes.Promocional;

    public void Apply(
        SaleItem item,
        decimal value,
        Guid? AuthorizerId = null,
        string? AuthorizerName = null,
        string? reason = null)
    {
        item.ApplyDiscount(AdditionDiscountTypes.Promocional, value, AuthorizerId, AuthorizerName, reason);
    }
}
