using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SalePaymentResult
{
    public Guid Id { get; set; }
    public Guid IdSales { get; set; }
    public PaymentType TypePayment { get; set; }
    public decimal Value { get; set; }
    public DateTime PaidAt { get; set; }
}
