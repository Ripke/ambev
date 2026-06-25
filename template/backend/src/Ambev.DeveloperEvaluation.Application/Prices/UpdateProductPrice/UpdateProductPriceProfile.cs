using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Prices.UpdateProductPrice;

public class UpdateProductPriceProfile : Profile
{
    public UpdateProductPriceProfile()
    {
        CreateMap<ProductPrice, UpdateProductPriceResult>();
    }
}
