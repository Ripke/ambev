using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductProfile : Profile
{
    public UpdateProductProfile()
    {
        CreateMap<UpdateProductRequest, Application.Products.UpdateProduct.UpdateProductCommand>();
        CreateMap<Application.Products.UpdateProduct.UpdateProductResult, UpdateProductResponse>();
    }
}
