using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItemQuantity;

public class UpdateSaleItemQuantityProfile : Profile
{
    public UpdateSaleItemQuantityProfile()
    {
        CreateMap<UpdateSaleItemQuantityRequest, Application.Sales.UpdateSaleItemQuantity.UpdateSaleItemQuantityCommand>();
        CreateMap<Application.Sales.UpdateSaleItemQuantity.UpdateSaleItemQuantityResult, UpdateSaleItemQuantityResponse>();
    }
}
