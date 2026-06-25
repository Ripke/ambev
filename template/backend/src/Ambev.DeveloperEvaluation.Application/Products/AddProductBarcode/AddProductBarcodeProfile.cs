using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.AddProductBarcode;

public class AddProductBarcodeProfile : Profile
{
    public AddProductBarcodeProfile()
    {
        CreateMap<ProductBarcode, AddProductBarcodeResult>();
    }
}
