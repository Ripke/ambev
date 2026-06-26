using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleItemProfile : Profile
{
    public SaleItemProfile()
    {
        CreateMap<SaleItem, SaleItemResult>();
        CreateMap<SalePayment, SalePaymentResult>();
        CreateMap<SaleChange, SaleChangeResult>();
        CreateMap<SalesItemDiscount, SaleItemDiscountResult>();
        CreateMap<SalesItemAddition, SaleItemAdditionResult>();
    }
}
