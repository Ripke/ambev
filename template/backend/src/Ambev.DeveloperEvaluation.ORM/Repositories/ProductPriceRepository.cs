using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductPriceRepository : IProductPriceRepository
{
    private readonly DefaultContext _context;

    public ProductPriceRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<ProductPrice> CreateAsync(ProductPrice price, CancellationToken cancellationToken = default)
    {
        await _context.ProductPrices.AddAsync(price, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return price;
    }

    public Task<ProductPrice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.ProductPrices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<ProductPrice?> GetCurrentPriceByProductIdAsync(Guid productId, PriceType priceType, DateTime asOf, CancellationToken cancellationToken = default)
    {
        return _context.ProductPrices
            .Where(x => x.ProductId == productId
                && x.PriceType == priceType
                && x.EffectiveStartAt <= asOf
                && asOf < x.EffectiveEndAt)
            .OrderByDescending(x => x.EffectiveStartAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductPrice>> ListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPrices
            .Where(x => x.ProductId == productId)
            .AsNoTracking()
            .OrderBy(x => x.EffectiveStartAt)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProductPrice price, CancellationToken cancellationToken = default)
    {
        _context.ProductPrices.Remove(price);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> HasOverlappingPriceAsync(
        Guid productId,
        PriceType priceType,
        DateTime effectiveStartAt,
        DateTime effectiveEndAt,
        Guid? ignorePriceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductPrices.AsQueryable()
            .Where(x => x.ProductId == productId &&
                        x.PriceType == priceType &&
                        x.EffectiveStartAt < effectiveEndAt &&
                        effectiveStartAt < x.EffectiveEndAt);

        if (ignorePriceId.HasValue)
            query = query.Where(x => x.Id != ignorePriceId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
