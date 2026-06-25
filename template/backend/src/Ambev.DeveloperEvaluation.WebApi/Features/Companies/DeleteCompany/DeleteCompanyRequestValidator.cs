using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.DeleteCompany;

public class DeleteCompanyRequestValidator : AbstractValidator<DeleteCompanyRequest>
{
    public DeleteCompanyRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
