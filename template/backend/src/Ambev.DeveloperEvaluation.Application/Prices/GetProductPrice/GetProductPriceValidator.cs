using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Prices.GetProductPrice;

public class GetProductPriceValidator : AbstractValidator<GetProductPriceCommand>
{
    public GetProductPriceValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
