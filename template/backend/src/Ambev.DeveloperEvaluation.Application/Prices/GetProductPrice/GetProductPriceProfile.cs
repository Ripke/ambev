using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Prices.GetProductPrice;

public class GetProductPriceProfile : Profile
{
    public GetProductPriceProfile()
    {
        CreateMap<ProductPrice, GetProductPriceResult>();
    }
}
