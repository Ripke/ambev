using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByUser;

public class GetCurrentSaleByUserProfile : Profile
{
    public GetCurrentSaleByUserProfile()
    {
        CreateMap<Sale, GetCurrentSaleByUserResult>();
    }
}
