using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.GetCompany;

public record GetCompanyCommand(Guid Id) : IRequest<GetCompanyResult>;
