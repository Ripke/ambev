using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Prices.ListProductPrices;

public record ListProductPricesCommand(Guid ProductId) : IRequest<IReadOnlyList<ListProductPricesResult>>;
