using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.GetCompany;

public class GetCompanyRequestValidator : AbstractValidator<GetCompanyRequest>
{
    public GetCompanyRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
