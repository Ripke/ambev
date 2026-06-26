using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionProfile : Profile
{
    public ApplyManualSaleItemAdditionProfile()
    {
        CreateMap<Sale, ApplyManualSaleItemAdditionResult>();
    }
}
