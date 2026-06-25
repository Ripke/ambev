using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.GetProductPrice;

public class GetProductPriceRequestValidator : AbstractValidator<GetProductPriceRequest>
{
    public GetProductPriceRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
