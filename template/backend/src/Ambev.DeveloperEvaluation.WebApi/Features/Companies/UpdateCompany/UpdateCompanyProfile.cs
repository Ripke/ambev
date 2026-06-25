using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.UpdateCompany;

public class UpdateCompanyProfile : Profile
{
    public UpdateCompanyProfile()
    {
        CreateMap<CompanyAddressRequest, Application.Companies.CompanyAddressCommand>();
        CreateMap<UpdateCompanyRequest, Application.Companies.UpdateCompany.UpdateCompanyCommand>();
        CreateMap<Application.Companies.CompanyAddressResult, CompanyAddressResponse>();
        CreateMap<Application.Companies.UpdateCompany.UpdateCompanyResult, UpdateCompanyResponse>();
    }
}
