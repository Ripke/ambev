using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.AddProductBarcode;

public class AddProductBarcodeRequestValidator : AbstractValidator<AddProductBarcodeRequest>
{
    public AddProductBarcodeRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Barcode).NotEmpty().MaximumLength(100);
    }
}
