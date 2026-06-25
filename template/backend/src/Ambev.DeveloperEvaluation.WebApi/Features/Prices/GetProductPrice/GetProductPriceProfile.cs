using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.GetProductPrice;

public class GetProductPriceProfile : Profile
{
    public GetProductPriceProfile()
    {
        CreateMap<Application.Prices.GetProductPrice.GetProductPriceResult, GetProductPriceResponse>();
        CreateMap<Guid, Application.Prices.GetProductPrice.GetProductPriceCommand>()
            .ConstructUsing(id => new Application.Prices.GetProductPrice.GetProductPriceCommand(id));
    }
}
