using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies.ListCompanies;

public class ListCompaniesProfile : Profile
{
    public ListCompaniesProfile()
    {
        CreateMap<Application.Companies.CompanyAddressResult, CompanyAddressResponse>();
        CreateMap<Application.Companies.ListCompanies.ListCompaniesResult, ListCompaniesResponse>();
    }
}
