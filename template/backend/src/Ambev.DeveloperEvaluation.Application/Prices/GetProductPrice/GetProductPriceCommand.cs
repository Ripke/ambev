using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Prices.GetProductPrice;

public record GetProductPriceCommand(Guid Id) : IRequest<GetProductPriceResult>;
