using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleRequest, Application.Sales.CreateSale.CreateSaleCommand>();
        CreateMap<Application.Sales.CreateSale.CreateSaleResult, CreateSaleResponse>();
    }
}
