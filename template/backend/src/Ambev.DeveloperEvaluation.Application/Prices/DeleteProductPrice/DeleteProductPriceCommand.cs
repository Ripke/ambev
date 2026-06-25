using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Prices.DeleteProductPrice;

public record DeleteProductPriceCommand(Guid Id) : IRequest<DeleteProductPriceResponse>;
