using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.DeleteCompany;

public class DeleteCompanyProfile : Profile
{
    public DeleteCompanyProfile()
    {
        CreateMap<Guid, Application.Companies.DeleteCompany.DeleteCompanyCommand>()
            .ConstructUsing(id => new Application.Companies.DeleteCompany.DeleteCompanyCommand(id));
    }
}
