using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.GetCustomer;

public record GetCustomerCommand(Guid Id) : IRequest<GetCustomerResult>;
