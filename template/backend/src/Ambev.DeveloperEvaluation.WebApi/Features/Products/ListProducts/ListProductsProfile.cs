using Ambev.DeveloperEvaluation.Application.Products;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public class ListProductsProfile : Profile
{
    public ListProductsProfile()
    {
        CreateMap<Application.Products.ListProducts.ListProductsResult, ListProductsResponse>();
        CreateMap<ProductBarcodeResult, ProductBarcodeResponse>();
    }
}
