using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions;

public class SalesPromotionProfile : Profile
{
    public SalesPromotionProfile()
    {
        CreateMap<Application.SalesPromotions.SalesPromotionItemResult, SalesPromotionItemResponse>();
        CreateMap<Application.SalesPromotions.CreateSalesPromotion.CreateSalesPromotionResult, CreateSalesPromotion.CreateSalesPromotionResponse>();
        CreateMap<Application.SalesPromotions.GetSalesPromotion.GetSalesPromotionResult, GetSalesPromotion.GetSalesPromotionResponse>();
        CreateMap<Application.SalesPromotions.ListSalesPromotions.ListSalesPromotionsResult, ListSalesPromotions.ListSalesPromotionsResponse>();
        CreateMap<Application.SalesPromotions.UpdateSalesPromotion.UpdateSalesPromotionResult, UpdateSalesPromotion.UpdateSalesPromotionResponse>();
        CreateMap<SalesPromotionItemRequest, Application.SalesPromotions.SalesPromotionItemInput>();
    }
}
