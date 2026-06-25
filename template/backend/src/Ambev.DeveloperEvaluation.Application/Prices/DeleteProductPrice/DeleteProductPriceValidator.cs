using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Prices.DeleteProductPrice;

public class DeleteProductPriceValidator : AbstractValidator<DeleteProductPriceCommand>
{
    public DeleteProductPriceValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
