using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Companies.UpdateCompany;

public class UpdateCompanyProfile : Profile
{
    public UpdateCompanyProfile()
    {
        CreateMap<CompanyAddress, CompanyAddressResult>();
        CreateMap<Company, UpdateCompanyResult>();
    }
}
