using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.AcrescimoDesconto;

public interface IServiceAdditionDiscount
{
    Task<Sale> Apply(
        Domain.Enums.AdditionDiscount tipoOperacao,
        AdditionDiscountTypes tipoAcrescimoDesconto,
        Guid saleId,
        Guid salesItemId,
        decimal valor,
        Guid? autorizadorId = null,
        string? motivo = null,
        CancellationToken cancellationToken = default);
}
