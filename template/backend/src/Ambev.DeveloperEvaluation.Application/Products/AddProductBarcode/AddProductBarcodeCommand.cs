using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.AddProductBarcode;

public class AddProductBarcodeCommand : IRequest<AddProductBarcodeResult>
{
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = string.Empty;

    public ValidationResultDetail Validate()
    {
        var validator = new AddProductBarcodeCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(x => (ValidationErrorDetail)x)
        };
    }
}
