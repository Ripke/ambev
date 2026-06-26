using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;

public interface IAdditionDiscountStrategy
{
    Domain.Enums.AdditionDiscount AdditionDiscount { get; }
    AdditionDiscountTypes AdditionDiscountType { get; }

    void Apply(
        SaleItem item,
        decimal value,
        Guid? AuthorizerId = null,
        string? AuthorizerName = null,
        string? reason = null);
}
