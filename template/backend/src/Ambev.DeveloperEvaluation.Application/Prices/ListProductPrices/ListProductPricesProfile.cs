using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Prices.ListProductPrices;

public class ListProductPricesProfile : Profile
{
    public ListProductPricesProfile()
    {
        CreateMap<ProductPrice, ListProductPricesResult>();
    }
}
