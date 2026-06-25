using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProductBarcode;

public class DeleteProductBarcodeValidator : AbstractValidator<DeleteProductBarcodeCommand>
{
    public DeleteProductBarcodeValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.BarcodeId).NotEmpty();
    }
}
