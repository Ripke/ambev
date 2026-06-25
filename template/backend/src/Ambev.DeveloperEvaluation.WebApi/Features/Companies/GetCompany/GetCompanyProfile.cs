using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.GetCompany;

public class GetCompanyProfile : Profile
{
    public GetCompanyProfile()
    {
        CreateMap<Application.Companies.CompanyAddressResult, CompanyAddressResponse>();
        CreateMap<Application.Companies.GetCompany.GetCompanyResult, GetCompanyResponse>();
        CreateMap<Guid, Application.Companies.GetCompany.GetCompanyCommand>()
            .ConstructUsing(id => new Application.Companies.GetCompany.GetCompanyCommand(id));
    }
}
