using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Sales.Promotions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.SubtotalizeSale;

public class SubtotalizeSaleHandler : IRequestHandler<SubtotalizeSaleCommand, SubtotalizeSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IPromotionalSaleService _promotionalSaleService;
    private readonly IMapper _mapper;

    public SubtotalizeSaleHandler(ISaleRepository saleRepository, IPromotionalSaleService promotionalSaleService, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _promotionalSaleService = promotionalSaleService;
        _mapper = mapper;
    }

    public async Task<SubtotalizeSaleResult> Handle(SubtotalizeSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new SubtotalizeSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        EnsureVersion(sale, request.Version);

        if (sale.Status != SaleStatus.Open)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.Id), "Only open sales can be subtotalized.")
            });
        }

        try
        {
            await _promotionalSaleService.ApplyAsync(sale, cancellationToken);
            sale.Subtotalize();
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.Id), ex.Message)
            });
        }

        await _saleRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubtotalizeSaleResult>(sale);
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
