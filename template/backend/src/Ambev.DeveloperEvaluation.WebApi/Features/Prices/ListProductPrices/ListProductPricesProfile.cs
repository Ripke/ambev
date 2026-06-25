using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.ListProductPrices;

public class ListProductPricesProfile : Profile
{
    public ListProductPricesProfile()
    {
        CreateMap<Application.Prices.ListProductPrices.ListProductPricesResult, ListProductPricesResponse>();
    }
}
