using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.GetSalesPromotion;

public class GetSalesPromotionProfile : Profile
{
    public GetSalesPromotionProfile()
    {
        CreateMap<Guid, Application.SalesPromotions.GetSalesPromotion.GetSalesPromotionCommand>()
            .ConstructUsing(id => new Application.SalesPromotions.GetSalesPromotion.GetSalesPromotionCommand(id));
    }
}
