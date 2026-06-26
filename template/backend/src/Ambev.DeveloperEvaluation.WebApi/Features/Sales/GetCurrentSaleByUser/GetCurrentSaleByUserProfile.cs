using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetCurrentSaleByUser;

public class GetCurrentSaleByUserProfile : Profile
{
    public GetCurrentSaleByUserProfile()
    {
        CreateMap<Application.Sales.GetCurrentSaleByUser.GetCurrentSaleByUserResult, GetCurrentSaleByUserResponse>();
        CreateMap<Guid, Application.Sales.GetCurrentSaleByUser.GetCurrentSaleByUserCommand>()
            .ConstructUsing(userId => new Application.Sales.GetCurrentSaleByUser.GetCurrentSaleByUserCommand(userId));
    }
}
