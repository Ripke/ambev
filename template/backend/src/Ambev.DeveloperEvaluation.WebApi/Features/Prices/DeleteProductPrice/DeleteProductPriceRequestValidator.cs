using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.DeleteProductPrice;

public class DeleteProductPriceRequestValidator : AbstractValidator<DeleteProductPriceRequest>
{
    public DeleteProductPriceRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
