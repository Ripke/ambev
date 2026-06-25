using Ambev.DeveloperEvaluation.Application.Products;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProductByBarcode;

public class GetProductByBarcodeProfile : Profile
{
    public GetProductByBarcodeProfile()
    {
        CreateMap<Application.Products.GetProductByBarcode.GetProductByBarcodeResult, GetProductByBarcodeResponse>();
        CreateMap<ProductBarcodeResult, ProductBarcodeResponse>();
        CreateMap<ProductPriceResult, ProductPriceResponse>();
        CreateMap<string, Application.Products.GetProductByBarcode.GetProductByBarcodeCommand>()
            .ConstructUsing(barcode => new Application.Products.GetProductByBarcode.GetProductByBarcodeCommand(barcode));
    }
}
