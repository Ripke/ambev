using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SalePaymentResult
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Value { get; set; }
    public DateTime PaidAt { get; set; }
}
