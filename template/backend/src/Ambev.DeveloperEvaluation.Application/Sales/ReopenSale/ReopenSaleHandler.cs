using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;

public class ReopenSaleHandler : IRequestHandler<ReopenSaleCommand, ReopenSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ReopenSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ReopenSaleResult> Handle(ReopenSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new ReopenSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        EnsureVersion(sale, request.Version);

        if (sale.Status != SaleStatus.Subtotalized)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.Id), "Only subtotalized sales can be reopened.")
            });
        }

        sale.Reopen();
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReopenSaleResult>(sale);
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
