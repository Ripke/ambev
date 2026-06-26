using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions;

public class SalesPromotionProfile : Profile
{
    public SalesPromotionProfile()
    {
        CreateMap<SalesPromotion, SalesPromotionResult>();
        CreateMap<SalesPromotion, CreateSalesPromotion.CreateSalesPromotionResult>();
        CreateMap<SalesPromotion, GetSalesPromotion.GetSalesPromotionResult>();
        CreateMap<SalesPromotion, ListSalesPromotions.ListSalesPromotionsResult>();
        CreateMap<SalesPromotion, UpdateSalesPromotion.UpdateSalesPromotionResult>();
        CreateMap<SalesPromotionItem, SalesPromotionItemResult>();
    }
}
