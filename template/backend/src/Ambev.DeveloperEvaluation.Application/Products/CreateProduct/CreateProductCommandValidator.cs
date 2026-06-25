using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UnitMeasure).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Brand).MaximumLength(100);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.ProductType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.MaxSaleQuantity).GreaterThan(0);
        RuleFor(x => x.Barcodes).NotEmpty();
        RuleForEach(x => x.Barcodes).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Barcodes)
            .Must(barcodes =>
            {
                var normalized = barcodes
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim());
                return normalized.Distinct(StringComparer.OrdinalIgnoreCase).Count() == normalized.Count();
            })
            .WithMessage("Barcodes must be unique.");
    }
}
