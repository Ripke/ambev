using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CancelSaleItemHandler(ISaleRepository saleRepository, IUserRepository userRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        if (sale.Status == SaleStatus.Subtotalized)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(sale.Status), "Subtotalized sale must be reopened before canceling an item.")
            });
        }

        EnsureSaleIsOpen(sale);

        var item = sale.GetItemOrThrow(request.ItemId);
        if (item.IsCanceled)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.ItemId), "Item is already canceled.")
            });
        }

        var authorizer = await _userRepository.GetByIdAsync(request.CancellationAuthorizerId, cancellationToken);
        if (authorizer == null)
            throw new KeyNotFoundException($"User with ID {request.CancellationAuthorizerId} not found");

        if (authorizer.Role != UserRole.Customer
            && authorizer.Role != UserRole.Manager
            && authorizer.Role != UserRole.Admin)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.CancellationAuthorizerId), "Cancellation authorizer must have Customer, Manager or Admin role.")
            });
        }

        sale.CancelItem(item.Id, request.CancellationAuthorizerId, authorizer.Username, request.CancellationReason);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CancelSaleItemResult>(sale);
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
