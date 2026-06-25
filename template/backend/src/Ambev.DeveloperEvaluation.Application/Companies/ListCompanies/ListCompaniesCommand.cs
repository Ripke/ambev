using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.ListCompanies;

public record ListCompaniesCommand : IRequest<IReadOnlyList<ListCompaniesResult>>;
