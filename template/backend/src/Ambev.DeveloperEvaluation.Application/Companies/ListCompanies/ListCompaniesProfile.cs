using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Companies.ListCompanies;

public class ListCompaniesProfile : Profile
{
    public ListCompaniesProfile()
    {
        CreateMap<CompanyAddress, CompanyAddressResult>();
        CreateMap<Company, ListCompaniesResult>();
    }
}
