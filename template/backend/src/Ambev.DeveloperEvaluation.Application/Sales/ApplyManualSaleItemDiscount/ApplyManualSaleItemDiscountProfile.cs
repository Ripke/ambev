using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountProfile : Profile
{
    public ApplyManualSaleItemDiscountProfile()
    {
        CreateMap<Sale, ApplyManualSaleItemDiscountResult>();
    }
}
