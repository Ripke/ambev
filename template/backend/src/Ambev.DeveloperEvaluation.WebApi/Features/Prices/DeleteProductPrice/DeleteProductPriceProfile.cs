using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices.DeleteProductPrice;

public class DeleteProductPriceProfile : Profile
{
    public DeleteProductPriceProfile()
    {
        CreateMap<Guid, Application.Prices.DeleteProductPrice.DeleteProductPriceCommand>()
            .ConstructUsing(id => new Application.Prices.DeleteProductPrice.DeleteProductPriceCommand(id));
    }
}
