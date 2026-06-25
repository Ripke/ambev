using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.CreateProductPrice;

public class CreateProductPriceProfile : Profile
{
    public CreateProductPriceProfile()
    {
        CreateMap<CreateProductPriceRequest, Application.Prices.CreateProductPrice.CreateProductPriceCommand>();
        CreateMap<Application.Prices.CreateProductPrice.CreateProductPriceResult, CreateProductPriceResponse>();
    }
}
