using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.UpdateSalesPromotion;

public class UpdateSalesPromotionProfile : Profile
{
    public UpdateSalesPromotionProfile()
    {
        CreateMap<UpdateSalesPromotionRequest, Application.SalesPromotions.UpdateSalesPromotion.UpdateSalesPromotionCommand>();
    }
}
