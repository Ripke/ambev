using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ReopenSale;

public class ReopenSaleProfile : Profile
{
    public ReopenSaleProfile()
    {
        CreateMap<ReopenSaleRequest, Application.Sales.ReopenSale.ReopenSaleCommand>();
        CreateMap<Application.Sales.ReopenSale.ReopenSaleResult, ReopenSaleResponse>();
    }
}
