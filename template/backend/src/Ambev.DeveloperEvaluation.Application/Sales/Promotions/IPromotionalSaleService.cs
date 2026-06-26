using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.Promotions;

public interface IPromotionalSaleService
{
    Task ApplyAsync(Sale sale, CancellationToken cancellationToken = default);
    void Clear(Sale sale);
}
