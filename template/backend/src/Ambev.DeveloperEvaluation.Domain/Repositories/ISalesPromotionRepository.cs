using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISalesPromotionRepository
{
    Task<SalesPromotion> CreateAsync(SalesPromotion promotion, CancellationToken cancellationToken = default);
    Task<SalesPromotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesPromotion>> ListAsync(CancellationToken cancellationToken = default);
    Task<SalesPromotionItem?> GetApplicableItemAsync(Guid productId, decimal quantity, DateTime referenceDate, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
