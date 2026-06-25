using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommand : IRequest<CancelSaleResult>
{
    public Guid Id { get; set; }
    public Guid Version { get; set; }
    public Guid CancellationAuthorizerId { get; set; }
    public string? CancellationReason { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new CancelSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
