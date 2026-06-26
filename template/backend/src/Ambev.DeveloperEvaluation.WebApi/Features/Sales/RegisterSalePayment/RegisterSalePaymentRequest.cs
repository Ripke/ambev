using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RegisterSalePayment;

public class RegisterSalePaymentRequest
{
    public Guid SaleId { get; set; }
    public Guid Version { get; set; }
    public PaymentType TypePayment { get; set; }
    public decimal Value { get; set; }
}
