using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Prices.UpdateProductPrice;

public class UpdateProductPriceCommand : IRequest<UpdateProductPriceResult>
{
    public Guid Id { get; set; }
    public PriceType PriceType { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveStartAt { get; set; }
    public DateTime EffectiveEndAt { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new UpdateProductPriceCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(x => (ValidationErrorDetail)x)
        };
    }
}
