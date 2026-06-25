using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.ListCustomers;

public class ListCustomersProfile : Profile
{
    public ListCustomersProfile()
    {
        CreateMap<Application.Customers.ListCustomers.ListCustomersResult, ListCustomersResponse>();
    }
}
