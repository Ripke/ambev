using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    public ProductRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Products
            .Include(x => x.Barcodes)
            .Include(x => x.Prices)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return _context.Products
            .Include(x => x.Barcodes)
            .Include(x => x.Prices)
            .FirstOrDefaultAsync(x => x.Barcodes.Any(b => b.Barcode == barcode), cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(x => x.Barcodes)
            .AsNoTracking()
            .OrderBy(x => x.Description)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Products.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> BarcodeExistsAsync(string barcode, Guid? ignoreBarcodeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProductBarcodes.AsQueryable()
            .Where(x => x.Barcode == barcode);

        if (ignoreBarcodeId.HasValue)
            query = query.Where(x => x.Id != ignoreBarcodeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public Task<ProductBarcode?> GetBarcodeByIdAsync(Guid productId, Guid barcodeId, CancellationToken cancellationToken = default)
    {
        return _context.ProductBarcodes
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.Id == barcodeId, cancellationToken);
    }

    public async Task DeleteBarcodeAsync(ProductBarcode barcode, CancellationToken cancellationToken = default)
    {
        _context.ProductBarcodes.Remove(barcode);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
