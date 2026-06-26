using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.RegisterSalePayment;

public class RegisterSalePaymentProfile : Profile
{
    public RegisterSalePaymentProfile()
    {
        CreateMap<Sale, RegisterSalePaymentResult>();
    }
}
