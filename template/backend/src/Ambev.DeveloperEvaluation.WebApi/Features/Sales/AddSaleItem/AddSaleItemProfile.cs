using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;

public class AddSaleItemProfile : Profile
{
    public AddSaleItemProfile()
    {
        CreateMap<AddSaleItemRequest, Application.Sales.AddSaleItem.AddSaleItemCommand>();
        CreateMap<Application.Sales.AddSaleItem.AddSaleItemResult, AddSaleItemResponse>();
    }
}
