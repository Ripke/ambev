using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountProfile : Profile
{
    public ApplyManualSaleItemDiscountProfile()
    {
        CreateMap<ApplyManualSaleItemDiscountRequest, Application.Sales.ApplyManualSaleItemDiscount.ApplyManualSaleItemDiscountCommand>();
        CreateMap<Application.Sales.ApplyManualSaleItemDiscount.ApplyManualSaleItemDiscountResult, ApplyManualSaleItemDiscountResponse>();
    }
}
