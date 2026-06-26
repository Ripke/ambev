using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;

public class ServiceAdditionDiscount : IServiceAdditionDiscount
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly Dictionary<(SaleItemAdjustmentKind, SaleItemAdjustmentType), IAdditionDiscountStrategy> _strategies;

    public ServiceAdditionDiscount(
        ISaleRepository saleRepository,
        IUserRepository userRepository,
        IEnumerable<IAdditionDiscountStrategy> strategies)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _strategies = strategies.ToDictionary(
            strategy => (strategy.AdjustmentKind, strategy.AdjustmentType),
            strategy => strategy);
    }

    public async Task<Sale> Apply(
        SaleItemAdjustmentKind operationKind,
        SaleItemAdjustmentType adjustmentType,
        Guid saleId,
        Guid saleItemId,
        decimal amount,
        Guid? authorizerId = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (!_strategies.TryGetValue((operationKind, adjustmentType), out var strategy))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(adjustmentType), $"Strategy not registered for {operationKind}/{adjustmentType}.")
            });
        }

        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {saleId} not found");

        EnsureSaleIsOpen(sale);

        var item = sale.GetItemOrThrow(saleItemId);
        if (item.IsCanceled)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(saleItemId), "Canceled item cannot be changed.")
            });
        }

        string? authorizerName = null;
        if (adjustmentType == SaleItemAdjustmentType.Manual)
        {
            if (authorizerId == null || authorizerId == Guid.Empty)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(authorizerId), "Manual addition/discount requires an authorizer.")
                });
            }

            var authorizer = await _userRepository.GetByIdAsync(authorizerId.Value, cancellationToken);
            if (authorizer == null)
                throw new KeyNotFoundException($"User with ID {authorizerId} not found");

            if (authorizer.Role != UserRole.Manager && authorizer.Role != UserRole.Admin)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(authorizerId), "Manual addition/discount authorizer must have Manager or Admin role.")
                });
            }

            authorizerName = authorizer.Username;
        }

        strategy.Apply(item, amount, authorizerId, authorizerName, reason);
        sale.RecalculateTotals();
        await _saleRepository.SaveChangesAsync(cancellationToken);
        return sale;
    }

    private static void EnsureSaleIsOpen(Sale sale)
    {
        if (sale.Status == SaleStatus.Open)
            return;

        throw new ValidationException(new[]
        {
            new ValidationFailure(nameof(sale.Status), "Items can only be changed while the sale is open.")
        });
    }
}
