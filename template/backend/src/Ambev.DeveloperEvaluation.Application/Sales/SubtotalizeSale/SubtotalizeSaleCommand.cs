using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.SubtotalizeSale;

public class SubtotalizeSaleCommand : IRequest<SubtotalizeSaleResult>
{
    public Guid Id { get; set; }
    public Guid Version { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new SubtotalizeSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
