using Ambev.DeveloperEvaluation.Application.Products;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductProfile : Profile
{
    public CreateProductProfile()
    {
        CreateMap<CreateProductRequest, Application.Products.CreateProduct.CreateProductCommand>();
        CreateMap<Application.Products.CreateProduct.CreateProductResult, CreateProductResponse>();
        CreateMap<ProductBarcodeResult, ProductBarcodeResponse>();
        CreateMap<ProductPriceResult, ProductPriceResponse>();
    }
}
