using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.SubtotalizeSale;

public class SubtotalizeSaleProfile : Profile
{
    public SubtotalizeSaleProfile()
    {
        CreateMap<SubtotalizeSaleRequest, Application.Sales.SubtotalizeSale.SubtotalizeSaleCommand>();
        CreateMap<Application.Sales.SubtotalizeSale.SubtotalizeSaleResult, SubtotalizeSaleResponse>();
    }
}
