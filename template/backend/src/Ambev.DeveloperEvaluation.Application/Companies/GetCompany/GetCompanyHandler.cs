using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Validation;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.GetCompany;

public class GetCompanyHandler : IRequestHandler<GetCompanyCommand, GetCompanyResult>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public GetCompanyHandler(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<GetCompanyResult> Handle(GetCompanyCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetCompanyValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (company == null)
            throw new KeyNotFoundException($"Company with ID {request.Id} not found");

        var result = _mapper.Map<GetCompanyResult>(company);
        result.Cnpj = CnpjHelper.Format(company.Cnpj);
        return result;
    }
}
