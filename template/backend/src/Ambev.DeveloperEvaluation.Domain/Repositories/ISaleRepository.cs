using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale?> GetCurrentByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Sale?> GetBySaleNumberAsync(long saleNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsOpenSaleByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
