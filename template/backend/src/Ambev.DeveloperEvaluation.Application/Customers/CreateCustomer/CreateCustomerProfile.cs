using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Customers.CreateCustomer;

public class CreateCustomerProfile : Profile
{
    public CreateCustomerProfile()
    {
        CreateMap<Customer, CreateCustomerResult>()
            .ForMember(destination => destination.MaskedCpf, options => options.Ignore());
    }
}
