using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.GetCustomer;

public class GetCustomerProfile : Profile
{
    public GetCustomerProfile()
    {
        CreateMap<Application.Customers.GetCustomer.GetCustomerResult, GetCustomerResponse>();
        CreateMap<Guid, Application.Customers.GetCustomer.GetCustomerCommand>()
            .ConstructUsing(id => new Application.Customers.GetCustomer.GetCustomerCommand(id));
    }
}
