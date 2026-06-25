using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.AddProductBarcode;

public class AddProductBarcodeProfile : Profile
{
    public AddProductBarcodeProfile()
    {
        CreateMap<AddProductBarcodeRequest, Application.Products.AddProductBarcode.AddProductBarcodeCommand>();
        CreateMap<Application.Products.AddProductBarcode.AddProductBarcodeResult, AddProductBarcodeResponse>();
    }
}
