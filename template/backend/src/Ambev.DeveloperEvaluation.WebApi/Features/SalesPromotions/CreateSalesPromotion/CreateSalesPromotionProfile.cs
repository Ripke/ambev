using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.CreateSalesPromotion;

public class CreateSalesPromotionProfile : Profile
{
    public CreateSalesPromotionProfile()
    {
        CreateMap<CreateSalesPromotionRequest, Application.SalesPromotions.CreateSalesPromotion.CreateSalesPromotionCommand>();
    }
}
