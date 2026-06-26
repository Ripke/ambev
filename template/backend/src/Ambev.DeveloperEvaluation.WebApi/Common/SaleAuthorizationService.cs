using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class SaleAuthorizationService : ISaleAuthorizationService
{
    private readonly ISaleRepository _saleRepository;

    public SaleAuthorizationService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public bool CanAccessUser(Guid targetUserId, Guid currentUserId, UserRole currentUserRole)
    {
        return currentUserRole != UserRole.Customer || targetUserId == currentUserId;
    }

    public async Task<bool> CanAccessSaleAsync(Guid saleId, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken)
    {
        if (currentUserRole != UserRole.Customer)
            return true;

        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        return sale == null || sale.UserId == currentUserId;
    }
}
