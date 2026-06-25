using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;

public class ReopenSaleCommand : IRequest<ReopenSaleResult>
{
    public Guid Id { get; set; }
    public Guid Version { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new ReopenSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
