using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItemQuantity;

public class UpdateSaleItemQuantityCommand : IRequest<UpdateSaleItemQuantityResult>
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new UpdateSaleItemQuantityCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
