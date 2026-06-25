using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Prices.ListProductPrices;

public class ListProductPricesHandler : IRequestHandler<ListProductPricesCommand, IReadOnlyList<ListProductPricesResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IMapper _mapper;

    public ListProductPricesHandler(
        IProductRepository productRepository,
        IProductPriceRepository productPriceRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _productPriceRepository = productPriceRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListProductPricesResult>> Handle(ListProductPricesCommand request, CancellationToken cancellationToken)
    {
        var validator = new ListProductPricesValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (!await _productRepository.ExistsAsync(request.ProductId, cancellationToken))
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");

        var prices = await _productPriceRepository.ListByProductIdAsync(request.ProductId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ListProductPricesResult>>(prices);
    }
}
