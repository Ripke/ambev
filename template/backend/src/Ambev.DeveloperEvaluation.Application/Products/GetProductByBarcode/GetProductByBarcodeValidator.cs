using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductByBarcode;

public class GetProductByBarcodeValidator : AbstractValidator<GetProductByBarcodeCommand>
{
    public GetProductByBarcodeValidator()
    {
        RuleFor(x => x.Barcode).NotEmpty().MaximumLength(100);
    }
}
