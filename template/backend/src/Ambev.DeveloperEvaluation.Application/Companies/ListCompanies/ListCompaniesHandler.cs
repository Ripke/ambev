using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Validation;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.ListCompanies;

public class ListCompaniesHandler : IRequestHandler<ListCompaniesCommand, IReadOnlyList<ListCompaniesResult>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public ListCompaniesHandler(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListCompaniesResult>> Handle(ListCompaniesCommand request, CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.ListAsync(cancellationToken);
        var results = _mapper.Map<IReadOnlyList<ListCompaniesResult>>(companies);

        for (var index = 0; index < companies.Count; index++)
        {
            results[index].Cnpj = CnpjHelper.Format(companies[index].Cnpj);
        }

        return results;
    }
}
