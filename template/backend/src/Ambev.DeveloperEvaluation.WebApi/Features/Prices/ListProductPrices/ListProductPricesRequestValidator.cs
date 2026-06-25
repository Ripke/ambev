using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.ListProductPrices;

public class ListProductPricesRequestValidator : AbstractValidator<ListProductPricesRequest>
{
    public ListProductPricesRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
