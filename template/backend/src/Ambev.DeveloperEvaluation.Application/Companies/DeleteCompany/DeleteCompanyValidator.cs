using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Companies.DeleteCompany;

public class DeleteCompanyValidator : AbstractValidator<DeleteCompanyCommand>
{
    public DeleteCompanyValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
