using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Companies.GetCompany;

public class GetCompanyValidator : AbstractValidator<GetCompanyCommand>
{
    public GetCompanyValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
