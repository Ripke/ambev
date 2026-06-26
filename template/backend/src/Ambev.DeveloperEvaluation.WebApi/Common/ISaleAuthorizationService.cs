using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public interface ISaleAuthorizationService
{
    bool CanAccessUser(Guid targetUserId, Guid currentUserId, UserRole currentUserRole);
    Task<bool> CanAccessSaleAsync(Guid saleId, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken);
}
