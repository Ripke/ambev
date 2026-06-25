using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.UpdateCompany;

public class UpdateCompanyHandler : IRequestHandler<UpdateCompanyCommand, UpdateCompanyResult>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public UpdateCompanyHandler(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<UpdateCompanyResult> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateCompanyCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (company == null)
            throw new KeyNotFoundException($"Company with ID {request.Id} not found");

        var normalizedCnpj = CnpjHelper.Normalize(request.Cnpj);
        var existingCompany = await _companyRepository.GetByCnpjAsync(normalizedCnpj, cancellationToken);
        if (existingCompany != null && existingCompany.Id != request.Id)
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.Cnpj), "CNPJ already exists.")
            });

        company.Update(
            request.Name,
            normalizedCnpj,
            new CompanyAddress(
                request.Address.Street,
                request.Address.Number,
                request.Address.Complement,
                request.Address.District,
                request.Address.City,
                request.Address.State,
                request.Address.ZipCode,
                request.Address.Country),
            request.Status);

        await _companyRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<UpdateCompanyResult>(company);
        result.Cnpj = CnpjHelper.Format(company.Cnpj);
        return result;
    }
}
