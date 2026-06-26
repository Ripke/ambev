using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.DeleteSalesPromotion;

public class DeleteSalesPromotionProfile : Profile
{
    public DeleteSalesPromotionProfile()
    {
        CreateMap<Guid, Application.SalesPromotions.DeleteSalesPromotion.DeleteSalesPromotionCommand>()
            .ConstructUsing(id => new Application.SalesPromotions.DeleteSalesPromotion.DeleteSalesPromotionCommand(id));
    }
}
