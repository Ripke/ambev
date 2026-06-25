using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;

public class AddSaleItemCommandValidator : AbstractValidator<AddSaleItemCommand>
{
    public AddSaleItemCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x)
            .Must(x => x.ProductId.HasValue ^ !string.IsNullOrWhiteSpace(x.Ean))
            .WithMessage("Either productId or ean must be provided, but not both.");
    }
}
