using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.RegisterSalePayment;

public class RegisterSalePaymentCommand : IRequest<RegisterSalePaymentResult>
{
    public Guid SaleId { get; set; }
    public Guid Version { get; set; }
    public PaymentType TypePayment { get; set; }
    public decimal Value { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new RegisterSalePaymentCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
