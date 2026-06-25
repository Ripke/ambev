using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UnitMeasure).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Brand).MaximumLength(100);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.ProductType).IsInEnum().Must(x => x != 0);
        RuleFor(x => x.MaxSaleQuantity).GreaterThan(0);
        RuleFor(x => x.Barcodes).NotEmpty();
        RuleForEach(x => x.Barcodes).NotEmpty().MaximumLength(100);
    }
}
