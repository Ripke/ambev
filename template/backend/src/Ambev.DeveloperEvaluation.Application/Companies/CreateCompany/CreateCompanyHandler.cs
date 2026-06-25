using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.CreateCompany;

public class CreateCompanyHandler : IRequestHandler<CreateCompanyCommand, CreateCompanyResult>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public CreateCompanyHandler(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<CreateCompanyResult> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCompanyCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var normalizedCnpj = CnpjHelper.Normalize(command.Cnpj);
        if (await _companyRepository.GetByCnpjAsync(normalizedCnpj, cancellationToken) != null)
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(command.Cnpj), "CNPJ already exists.")
            });

        var company = Domain.Entities.Company.Create(
            command.Name,
            normalizedCnpj,
            new CompanyAddress(
                command.Address.Street,
                command.Address.Number,
                command.Address.Complement,
                command.Address.District,
                command.Address.City,
                command.Address.State,
                command.Address.ZipCode,
                command.Address.Country),
            command.Status);

        var createdCompany = await _companyRepository.CreateAsync(company, cancellationToken);
        var result = _mapper.Map<CreateCompanyResult>(createdCompany);
        result.Cnpj = CnpjHelper.Format(createdCompany.Cnpj);
        return result;
    }
}
