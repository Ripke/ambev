using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItemQuantity;

public class UpdateSaleItemQuantityProfile : Profile
{
    public UpdateSaleItemQuantityProfile()
    {
        CreateMap<Sale, UpdateSaleItemQuantityResult>();
    }
}
