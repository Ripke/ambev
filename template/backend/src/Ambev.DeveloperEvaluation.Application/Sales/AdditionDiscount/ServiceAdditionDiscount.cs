using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Ambev.DeveloperEvaluation.Application.Sales.AcrescimoDesconto;

public class ServiceAdditionDiscount : IServiceAdditionDiscount
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly Dictionary<(Domain.Enums.AdditionDiscount, AdditionDiscountTypes), IAdditionDiscountStrategy> _strategies;

    public ServiceAdditionDiscount(
        ISaleRepository saleRepository,
        IUserRepository userRepository,
        IEnumerable<IAdditionDiscountStrategy> strategies)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _strategies = strategies.ToDictionary(
            strategy => (strategy.AdditionDiscount, strategy.AdditionDiscountType),
            strategy => strategy);
    }

    public async Task<Sale> Apply(
        Domain.Enums.AdditionDiscount operationType,
        AdditionDiscountTypes additionDiscountType,
        Guid saleId,
        Guid salesItemId,
        decimal value,
        Guid? AuthorizerId = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (!_strategies.TryGetValue((operationType, additionDiscountType), out var strategy))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(additionDiscountType), $"Strategy not registered for {operationType}/{additionDiscountType}.")
            });
        }

        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {saleId} not found");

        EnsureSaleIsOpen(sale);

        var item = sale.GetItemOrThrow(salesItemId);
        if (item.IsCanceled)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(salesItemId), "Canceled item cannot be changed.")
            });
        }

        string? authorizerName = null;
        if (additionDiscountType == AdditionDiscountTypes.Manual)
        {
            if (AuthorizerId == null || AuthorizerId == Guid.Empty)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(AuthorizerId), "Manual addition/discount requires an authorizer.")
                });
            }

            var authorizer = await _userRepository.GetByIdAsync(AuthorizerId.Value, cancellationToken);
            if (authorizer == null)
                throw new KeyNotFoundException($"User with ID {AuthorizerId} not found");

            if (authorizer.Role != UserRole.Manager && authorizer.Role != UserRole.Admin)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(AuthorizerId), "Manual addition/discount authorizer must have Manager or Admin role.")
                });
            }

            authorizerName = authorizer.Username;
        }

        strategy.Apply(item, value, AuthorizerId, authorizerName, reason);
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
