using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.GetSalesPromotion;

public record GetSalesPromotionCommand(Guid Id) : IRequest<GetSalesPromotionResult>;
