using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SalePaymentResponse
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Value { get; set; }
    public DateTime PaidAt { get; set; }
}
