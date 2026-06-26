using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SaleChangeResponse
{
    public Guid Id { get; set; }
    public Guid IdSales { get; set; }
    public PaymentType TypePayment { get; set; }
    public decimal Value { get; set; }
    public DateTime ChangedAt { get; set; }
}
