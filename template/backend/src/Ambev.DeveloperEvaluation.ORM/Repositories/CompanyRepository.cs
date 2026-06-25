using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly DefaultContext _context;

    public CompanyRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Company> CreateAsync(Company company, CancellationToken cancellationToken = default)
    {
        await _context.Companies.AddAsync(company, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return company;
    }

    public Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Companies.FirstOrDefaultAsync(company => company.Id == id, cancellationToken);
    }

    public Task<Company?> GetByCnpjAsync(string normalizedCnpj, CancellationToken cancellationToken = default)
    {
        return _context.Companies.FirstOrDefaultAsync(company => company.Cnpj == normalizedCnpj, cancellationToken);
    }

    public async Task<IReadOnlyList<Company>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .AsNoTracking()
            .OrderBy(company => company.Name)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
