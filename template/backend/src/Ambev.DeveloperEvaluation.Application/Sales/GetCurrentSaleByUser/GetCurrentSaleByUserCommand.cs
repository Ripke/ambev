using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByUser;

public record GetCurrentSaleByUserCommand(Guid UserId) : IRequest<GetCurrentSaleByUserResult>;
