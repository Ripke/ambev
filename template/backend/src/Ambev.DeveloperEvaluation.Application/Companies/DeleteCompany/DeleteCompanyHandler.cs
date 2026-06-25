using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Companies.DeleteCompany;

public class DeleteCompanyHandler : IRequestHandler<DeleteCompanyCommand, DeleteCompanyResponse>
{
    private readonly ICompanyRepository _companyRepository;

    public DeleteCompanyHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<DeleteCompanyResponse> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteCompanyValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (company == null)
            throw new KeyNotFoundException($"Company with ID {request.Id} not found");

        company.Deactivate();
        await _companyRepository.SaveChangesAsync(cancellationToken);

        return new DeleteCompanyResponse { Success = true };
    }
}
