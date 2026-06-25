using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.CreateCompany;

public class CreateCompanyProfile : Profile
{
    public CreateCompanyProfile()
    {
        CreateMap<CompanyAddressRequest, Application.Companies.CompanyAddressCommand>();
        CreateMap<CreateCompanyRequest, Application.Companies.CreateCompany.CreateCompanyCommand>();
        CreateMap<Application.Companies.CompanyAddressResult, CompanyAddressResponse>();
        CreateMap<Application.Companies.CreateCompany.CreateCompanyResult, CreateCompanyResponse>();
    }
}
