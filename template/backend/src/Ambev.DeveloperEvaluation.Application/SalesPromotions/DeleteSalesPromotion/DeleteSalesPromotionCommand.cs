using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.DeleteSalesPromotion;

public record DeleteSalesPromotionCommand(Guid Id) : IRequest<DeleteSalesPromotionResponse>;
