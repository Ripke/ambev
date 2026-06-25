using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.DeleteCompany;

public record DeleteCompanyCommand(Guid Id) : IRequest<DeleteCompanyResponse>;
