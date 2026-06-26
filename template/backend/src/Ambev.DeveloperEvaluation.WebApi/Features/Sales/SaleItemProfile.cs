using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SaleItemProfile : Profile
{
    public SaleItemProfile()
    {
        CreateMap<Application.Sales.SaleItemResult, SaleItemResponse>();
        CreateMap<Application.Sales.SalePaymentResult, SalePaymentResponse>();
        CreateMap<Application.Sales.SaleChangeResult, SaleChangeResponse>();
        CreateMap<Application.Sales.SaleItemDiscountResult, SaleItemDiscountResponse>();
        CreateMap<Application.Sales.SaleItemAdditionResult, SaleItemAdditionResponse>();
    }
}
