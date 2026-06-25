using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;

public class AddSaleItemCommand : IRequest<AddSaleItemResult>
{
    public Guid SaleId { get; set; }
    public Guid? ProductId { get; set; }
    public string? Ean { get; set; }
    public decimal Quantity { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new AddSaleItemCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
