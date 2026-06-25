using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.AddProductBarcode;

public class AddProductBarcodeCommandValidator : AbstractValidator<AddProductBarcodeCommand>
{
    public AddProductBarcodeCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Barcode).NotEmpty().MaximumLength(100);
    }
}
