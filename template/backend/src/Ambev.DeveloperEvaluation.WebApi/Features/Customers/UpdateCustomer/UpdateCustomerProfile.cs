using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.UpdateCustomer;

public class UpdateCustomerProfile : Profile
{
    public UpdateCustomerProfile()
    {
        CreateMap<UpdateCustomerRequest, Application.Customers.UpdateCustomer.UpdateCustomerCommand>();
        CreateMap<Application.Customers.UpdateCustomer.UpdateCustomerResult, UpdateCustomerResponse>();
    }
}
