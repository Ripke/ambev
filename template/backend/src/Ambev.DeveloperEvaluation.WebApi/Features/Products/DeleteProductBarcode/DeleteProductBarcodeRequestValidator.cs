using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProductBarcode;

public class DeleteProductBarcodeRequestValidator : AbstractValidator<DeleteProductBarcodeRequest>
{
    public DeleteProductBarcodeRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.BarcodeId).NotEmpty();
    }
}
