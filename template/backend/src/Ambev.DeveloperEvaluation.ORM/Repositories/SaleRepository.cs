using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Sales
            .Include(sale => sale.Items.OrderBy(item => item.SequentialNumber))
                .ThenInclude(item => item.Discounts)
            .Include(sale => sale.Items.OrderBy(item => item.SequentialNumber))
                .ThenInclude(item => item.Additions)
            .Include(sale => sale.Payments.OrderBy(payment => payment.PaidAt))
            .Include(sale => sale.Changes.OrderBy(change => change.ChangedAt))
            .FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

    public Task<Sale?> GetCurrentByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.Sales
            .Include(sale => sale.Items.OrderBy(item => item.SequentialNumber))
                .ThenInclude(item => item.Discounts)
            .Include(sale => sale.Items.OrderBy(item => item.SequentialNumber))
                .ThenInclude(item => item.Additions)
            .Include(sale => sale.Payments.OrderBy(payment => payment.PaidAt))
            .Include(sale => sale.Changes.OrderBy(change => change.ChangedAt))
            .Where(sale => sale.UserId == userId
                && sale.Status != SaleStatus.IntegratedWithErp
                && sale.Status != SaleStatus.Canceled)
            .OrderByDescending(sale => sale.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Sale?> GetBySaleNumberAsync(long saleNumber, CancellationToken cancellationToken = default)
    {
        return _context.Sales
            .Include(sale => sale.Items.OrderBy(item => item.SequentialNumber))
                .ThenInclude(item => item.Discounts)
            .Include(sale => sale.Items.OrderBy(item => item.SequentialNumber))
                .ThenInclude(item => item.Additions)
            .Include(sale => sale.Payments.OrderBy(payment => payment.PaidAt))
            .Include(sale => sale.Changes.OrderBy(change => change.ChangedAt))
            .FirstOrDefaultAsync(sale => sale.SaleNumber == saleNumber, cancellationToken);
    }

    public Task<bool> ExistsOpenSaleByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.Sales.AnyAsync(sale =>
            sale.UserId == userId
            && sale.Status != SaleStatus.IntegratedWithErp
            && sale.Status != SaleStatus.Canceled, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
