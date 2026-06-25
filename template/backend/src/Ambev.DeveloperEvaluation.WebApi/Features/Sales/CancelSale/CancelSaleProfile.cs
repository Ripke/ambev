using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public class CancelSaleProfile : Profile
{
    public CancelSaleProfile()
    {
        CreateMap<CancelSaleRequest, Application.Sales.CancelSale.CancelSaleCommand>();
        CreateMap<Application.Sales.CancelSale.CancelSaleResult, CancelSaleResponse>();
    }
}
