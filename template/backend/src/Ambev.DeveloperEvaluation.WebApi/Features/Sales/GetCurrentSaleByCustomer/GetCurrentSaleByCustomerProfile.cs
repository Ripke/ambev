using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetCurrentSaleByCustomer;

public class GetCurrentSaleByCustomerProfile : Profile
{
    public GetCurrentSaleByCustomerProfile()
    {
        CreateMap<Application.Sales.GetCurrentSaleByCustomer.GetCurrentSaleByCustomerResult, GetCurrentSaleByCustomerResponse>();
        CreateMap<Guid, Application.Sales.GetCurrentSaleByCustomer.GetCurrentSaleByCustomerCommand>()
            .ConstructUsing(customerId => new Application.Sales.GetCurrentSaleByCustomer.GetCurrentSaleByCustomerCommand(customerId));
    }
}
