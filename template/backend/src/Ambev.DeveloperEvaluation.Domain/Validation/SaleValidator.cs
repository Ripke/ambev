using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.CompanyId).NotEmpty();
        RuleFor(sale => sale.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(sale => sale.UserId).NotEmpty();
        RuleFor(sale => sale.UserName).NotEmpty().MaximumLength(200);
        RuleFor(sale => sale.Version).NotEmpty();
        RuleFor(sale => sale.Status)
            .IsInEnum()
            .Must(status => status != SaleStatus.Unknown);
        RuleFor(sale => sale.Subtotal).GreaterThanOrEqualTo(0);
        RuleFor(sale => sale.Total).GreaterThanOrEqualTo(0);
        RuleFor(sale => sale.AdditionalAmountTotal).GreaterThanOrEqualTo(0);
        RuleFor(sale => sale.DiscountAmountTotal).GreaterThanOrEqualTo(0);
        RuleFor(sale => sale.PaymentAmountTotal).GreaterThanOrEqualTo(0);
        RuleFor(sale => sale.ChangeAmountTotal).GreaterThanOrEqualTo(0);
        RuleFor(sale => sale.CancellationAuthorizerName)
            .MaximumLength(200);
        RuleFor(sale => sale.CancellationReason)
            .MaximumLength(500);
    }
}
