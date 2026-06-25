using Ambev.DeveloperEvaluation.Application.Products;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;

public class GetProductProfile : Profile
{
    public GetProductProfile()
    {
        CreateMap<Application.Products.GetProduct.GetProductResult, GetProductResponse>();
        CreateMap<ProductBarcodeResult, ProductBarcodeResponse>();
        CreateMap<ProductPriceResult, ProductPriceResponse>();
        CreateMap<Guid, Application.Products.GetProduct.GetProductCommand>()
            .ConstructUsing(id => new Application.Products.GetProduct.GetProductCommand(id));
    }
}
