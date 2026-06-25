using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Prices.CreateProductPrice;

public class CreateProductPriceHandler : IRequestHandler<CreateProductPriceCommand, CreateProductPriceResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IMapper _mapper;

    public CreateProductPriceHandler(
        IProductRepository productRepository,
        IProductPriceRepository productPriceRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _productPriceRepository = productPriceRepository;
        _mapper = mapper;
    }

    public async Task<CreateProductPriceResult> Handle(CreateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateProductPriceCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (!await _productRepository.ExistsAsync(request.ProductId, cancellationToken))
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");

        var hasOverlap = await _productPriceRepository.HasOverlappingPriceAsync(
            request.ProductId,
            request.PriceType,
            request.EffectiveStartAt,
            request.EffectiveEndAt,
            cancellationToken: cancellationToken);

        if (hasOverlap)
            throw new InvalidOperationException("There is already a price for this product and price type in the informed period.");

        var price = Domain.Entities.ProductPrice.Create(
            request.ProductId,
            request.PriceType,
            request.Price,
            request.EffectiveStartAt,
            request.EffectiveEndAt);

        try
        {
            var createdPrice = await _productPriceRepository.CreateAsync(price, cancellationToken);
            return _mapper.Map<CreateProductPriceResult>(createdPrice);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("EX_ProductPrices_NoOverlap", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException("There is already a price for this product and price type in the informed period.");
        }
    }
}
