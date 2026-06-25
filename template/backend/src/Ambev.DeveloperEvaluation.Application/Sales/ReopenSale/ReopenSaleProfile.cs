using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;

public class ReopenSaleProfile : Profile
{
    public ReopenSaleProfile()
    {
        CreateMap<Sale, ReopenSaleResult>();
    }
}
