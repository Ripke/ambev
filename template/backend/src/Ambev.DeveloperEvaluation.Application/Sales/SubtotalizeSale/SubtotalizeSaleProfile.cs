using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.SubtotalizeSale;

public class SubtotalizeSaleProfile : Profile
{
    public SubtotalizeSaleProfile()
    {
        CreateMap<Sale, SubtotalizeSaleResult>();
    }
}
