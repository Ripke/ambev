using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ICompanyRepository
{
    Task<Company> CreateAsync(Company company, CancellationToken cancellationToken = default);
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company?> GetByCnpjAsync(string normalizedCnpj, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Company>> ListAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
