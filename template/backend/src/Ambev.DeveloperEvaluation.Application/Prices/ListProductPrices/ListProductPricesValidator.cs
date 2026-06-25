using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Prices.ListProductPrices;

public class ListProductPricesValidator : AbstractValidator<ListProductPricesCommand>
{
    public ListProductPricesValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
