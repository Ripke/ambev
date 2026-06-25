using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.ListCustomers;

public record ListCustomersCommand : IRequest<IReadOnlyList<ListCustomersResult>>;
