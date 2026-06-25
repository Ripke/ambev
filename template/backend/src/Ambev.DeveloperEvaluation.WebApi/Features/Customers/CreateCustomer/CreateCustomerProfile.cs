using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.CreateCustomer;

public class CreateCustomerProfile : Profile
{
    public CreateCustomerProfile()
    {
        CreateMap<CreateCustomerRequest, Application.Customers.CreateCustomer.CreateCustomerCommand>();
        CreateMap<Application.Customers.CreateCustomer.CreateCustomerResult, CreateCustomerResponse>();
    }
}
