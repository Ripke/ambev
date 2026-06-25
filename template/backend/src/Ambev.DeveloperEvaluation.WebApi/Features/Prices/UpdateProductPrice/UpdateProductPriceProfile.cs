using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.UpdateProductPrice;

public class UpdateProductPriceProfile : Profile
{
    public UpdateProductPriceProfile()
    {
        CreateMap<UpdateProductPriceRequest, Application.Prices.UpdateProductPrice.UpdateProductPriceCommand>();
        CreateMap<Application.Prices.UpdateProductPrice.UpdateProductPriceResult, UpdateProductPriceResponse>();
    }
}
