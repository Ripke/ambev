using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.RegisterSalePayment;

public class RegisterSalePaymentHandler : IRequestHandler<RegisterSalePaymentCommand, RegisterSalePaymentResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public RegisterSalePaymentHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<RegisterSalePaymentResult> Handle(RegisterSalePaymentCommand request, CancellationToken cancellationToken)
    {
        var validator = new RegisterSalePaymentCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        EnsureVersion(sale, request.Version);
        EnsureStatus(sale.Status);

        try
        {
            sale.RegisterPayment(request.TypePayment, request.Value);
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.SaleId), ex.Message)
            });
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(ex.ParamName ?? nameof(request.Value), ex.Message)
            });
        }

        await _saleRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RegisterSalePaymentResult>(sale);
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

    private static void EnsureStatus(SaleStatus status)
    {
        if (status == SaleStatus.Subtotalized || status == SaleStatus.PaymentCompleted)
            return;

        throw new ValidationException(new[]
        {
            new ValidationFailure(nameof(status), "Payments can only be registered for subtotalized or payment pending sales.")
        });
    }
}
