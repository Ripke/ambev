using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Companies.GetCompany;

public class GetCompanyProfile : Profile
{
    public GetCompanyProfile()
    {
        CreateMap<CompanyAddress, CompanyAddressResult>();
        CreateMap<Company, GetCompanyResult>();
    }
}
