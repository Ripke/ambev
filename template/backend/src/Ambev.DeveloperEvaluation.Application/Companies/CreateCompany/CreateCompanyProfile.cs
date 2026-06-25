using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Companies.CreateCompany;

public class CreateCompanyProfile : Profile
{
    public CreateCompanyProfile()
    {
        CreateMap<CompanyAddress, CompanyAddressResult>();
        CreateMap<Company, CreateCompanyResult>();
    }
}
