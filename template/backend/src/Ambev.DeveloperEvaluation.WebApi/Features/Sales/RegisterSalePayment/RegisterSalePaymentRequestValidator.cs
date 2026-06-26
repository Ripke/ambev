using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RegisterSalePayment;

public class RegisterSalePaymentRequestValidator : AbstractValidator<RegisterSalePaymentRequest>
{
    public RegisterSalePaymentRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
        RuleFor(x => x.PaymentType)
            .IsInEnum()
            .NotEqual(PaymentType.Unknown);
        RuleFor(x => x.Value).GreaterThan(0);
    }
}
