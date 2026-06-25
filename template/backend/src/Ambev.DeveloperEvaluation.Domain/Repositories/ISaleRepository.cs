using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale?> GetCurrentByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Sale?> GetBySaleNumberAsync(long saleNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsOpenSaleByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
