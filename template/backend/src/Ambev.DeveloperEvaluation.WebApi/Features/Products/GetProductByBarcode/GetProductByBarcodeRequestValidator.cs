using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProductByBarcode;

public class GetProductByBarcodeRequestValidator : AbstractValidator<GetProductByBarcodeRequest>
{
    public GetProductByBarcodeRequestValidator()
    {
        RuleFor(x => x.Barcode).NotEmpty().MaximumLength(100);
    }
}
