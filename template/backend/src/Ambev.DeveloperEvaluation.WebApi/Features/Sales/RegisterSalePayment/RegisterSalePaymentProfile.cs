using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RegisterSalePayment;

public class RegisterSalePaymentProfile : Profile
{
    public RegisterSalePaymentProfile()
    {
        CreateMap<RegisterSalePaymentRequest, Application.Sales.RegisterSalePayment.RegisterSalePaymentCommand>();
        CreateMap<Application.Sales.RegisterSalePayment.RegisterSalePaymentResult, RegisterSalePaymentResponse>();
    }
}
