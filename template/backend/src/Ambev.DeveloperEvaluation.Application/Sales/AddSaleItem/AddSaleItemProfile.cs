using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;

public class AddSaleItemProfile : Profile
{
    public AddSaleItemProfile()
    {
        CreateMap<Sale, AddSaleItemResult>();
    }
}
