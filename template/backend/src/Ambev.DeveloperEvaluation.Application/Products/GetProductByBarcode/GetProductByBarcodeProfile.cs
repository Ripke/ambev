using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductByBarcode;

public class GetProductByBarcodeProfile : Profile
{
    public GetProductByBarcodeProfile()
    {
        CreateMap<Product, GetProductByBarcodeResult>();
        CreateMap<ProductBarcode, ProductBarcodeResult>();
        CreateMap<ProductPrice, ProductPriceResult>();
    }
}
