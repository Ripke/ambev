using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.RegisterSalePayment;

public class RegisterSalePaymentCommandValidator : AbstractValidator<RegisterSalePaymentCommand>
{
    public RegisterSalePaymentCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
        RuleFor(x => x.PaymentType)
            .IsInEnum()
            .NotEqual(PaymentType.Unknown);
        RuleFor(x => x.Value).GreaterThan(0);
    }
}
