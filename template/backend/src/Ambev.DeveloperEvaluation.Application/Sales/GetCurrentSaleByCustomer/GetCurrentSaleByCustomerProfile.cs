using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByCustomer;

public class GetCurrentSaleByCustomerProfile : Profile
{
    public GetCurrentSaleByCustomerProfile()
    {
        CreateMap<Sale, GetCurrentSaleByCustomerResult>();
    }
}
