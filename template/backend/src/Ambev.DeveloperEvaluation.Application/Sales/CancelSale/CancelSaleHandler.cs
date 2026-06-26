using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CancelSaleHandler(ISaleRepository saleRepository, IUserRepository userRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        EnsureVersion(sale, request.Version);

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

        if (sale.Status != SaleStatus.Open
            && sale.Status != SaleStatus.Subtotalized
            && sale.Status != SaleStatus.PaymentCompleted)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.Id), $"Sale in status {sale.Status} cannot be canceled.")
            });
        }

        sale.Cancel(request.CancellationAuthorizerId, authorizer.Username, request.CancellationReason);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CancelSaleResult>(sale);
    }

    private static void EnsureVersion(Domain.Entities.Sale sale, Guid version)
    {
        if (sale.MatchesVersion(version))
            return;

        throw new ValidationException(new[]
        {
            new ValidationFailure(nameof(version), "Sale version is outdated.")
        });
    }
}
