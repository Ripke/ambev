using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProductBarcode;

public class DeleteProductBarcodeProfile : Profile
{
    public DeleteProductBarcodeProfile()
    {
        CreateMap<DeleteProductBarcodeRequest, Application.Products.DeleteProductBarcode.DeleteProductBarcodeCommand>()
            .ConstructUsing(request => new Application.Products.DeleteProductBarcode.DeleteProductBarcodeCommand(request.ProductId, request.BarcodeId));
    }
}
