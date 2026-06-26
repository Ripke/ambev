using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;

public class AdditionManualStrategy : IAdditionDiscountStrategy
{
    public Domain.Enums.AdditionDiscount AdditionDiscount => Domain.Enums.AdditionDiscount.Acrescimo;
    public AdditionDiscountTypes AdditionDiscountType => AdditionDiscountTypes.Manual;

    public void Apply(
        SaleItem item,
        decimal value,
        Guid? AuthorizerId = null,
        string? AuthorizerName = null,
        string? reason = null)
    {
        item.ApplyAddition(AdditionDiscountTypes.Manual, value, AuthorizerId, AuthorizerName, reason);
    }
}
