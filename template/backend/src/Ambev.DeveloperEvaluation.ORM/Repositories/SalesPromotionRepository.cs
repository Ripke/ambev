using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SalesPromotionRepository : ISalesPromotionRepository
{
    private readonly DefaultContext _context;

    public SalesPromotionRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<SalesPromotion> CreateAsync(SalesPromotion promotion, CancellationToken cancellationToken = default)
    {
        await _context.SalesPromotions.AddAsync(promotion, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return promotion;
    }

    public Task<SalesPromotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.SalesPromotions
            .Include(x => x.Items.OrderBy(item => item.MinimumQuantity))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<SalesPromotion>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SalesPromotions
            .Include(x => x.Items.OrderBy(item => item.MinimumQuantity))
            .AsNoTracking()
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<SalesPromotionItem?> GetApplicableItemAsync(Guid productId, decimal quantity, DateTime referenceDate, CancellationToken cancellationToken = default)
    {
        return _context.SalesPromotionItems
            .Include(item => item.Promotion)
            .Where(item =>
                item.Promotion.IsActive &&
                item.Promotion.StartDate <= referenceDate &&
                item.Promotion.EndDate >= referenceDate &&
                (item.Promotion.ProductId == null || item.Promotion.ProductId == productId) &&
                item.MinimumQuantity <= quantity &&
                item.MaximumQuantity >= quantity)
            .OrderByDescending(item => item.Promotion.Priority)
            .ThenBy(item => item.Promotion.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
