using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByCustomer;

public record GetCurrentSaleByCustomerCommand(Guid CustomerId) : IRequest<GetCurrentSaleByCustomerResult>;
