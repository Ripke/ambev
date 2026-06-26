using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionProfile : Profile
{
    public ApplyManualSaleItemAdditionProfile()
    {
        CreateMap<ApplyManualSaleItemAdditionRequest, Application.Sales.ApplyManualSaleItemAddition.ApplyManualSaleItemAdditionCommand>();
        CreateMap<Application.Sales.ApplyManualSaleItemAddition.ApplyManualSaleItemAdditionResult, ApplyManualSaleItemAdditionResponse>();
    }
}
