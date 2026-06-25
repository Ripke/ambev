using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IProductPriceRepository
{
    Task<ProductPrice> CreateAsync(ProductPrice price, CancellationToken cancellationToken = default);
    Task<ProductPrice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductPrice>> ListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task DeleteAsync(ProductPrice price, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingPriceAsync(
        Guid productId,
        PriceType priceType,
        DateTime effectiveStartAt,
        DateTime effectiveEndAt,
        Guid? ignorePriceId = null,
        CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
