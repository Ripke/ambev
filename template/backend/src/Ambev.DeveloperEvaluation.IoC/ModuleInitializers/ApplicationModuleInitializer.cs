using Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;
using Ambev.DeveloperEvaluation.Common.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddSingleton<ICpfProtectionService, AesCpfProtectionService>();
        builder.Services.AddScoped<IServiceAdditionDiscount, ServiceAdditionDiscount>();
        builder.Services.AddScoped<IAdditionDiscountStrategy, AdditionManualStrategy>();
        builder.Services.AddScoped<IAdditionDiscountStrategy, DiscountManualStrategy>();
        builder.Services.AddScoped<IAdditionDiscountStrategy, DiscountPromotionalStrategy>();
    }
}
