using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Customers.UpdateCustomer;

public class UpdateCustomerProfile : Profile
{
    public UpdateCustomerProfile()
    {
        CreateMap<Customer, UpdateCustomerResult>()
            .ForMember(destination => destination.MaskedCpf, options => options.Ignore());
    }
}
