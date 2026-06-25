using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Prices.CreateProductPrice;

public class CreateProductPriceProfile : Profile
{
    public CreateProductPriceProfile()
    {
        CreateMap<ProductPrice, CreateProductPriceResult>();
    }
}
