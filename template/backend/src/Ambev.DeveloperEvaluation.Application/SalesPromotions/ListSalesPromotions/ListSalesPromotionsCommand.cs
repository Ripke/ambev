using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.ListSalesPromotions;

public record ListSalesPromotionsCommand() : IRequest<IReadOnlyList<ListSalesPromotionsResult>>;
