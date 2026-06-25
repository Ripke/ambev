using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Prices.UpdateProductPrice;

public class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceCommand, UpdateProductPriceResult>
{
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IMapper _mapper;

    public UpdateProductPriceHandler(IProductPriceRepository productPriceRepository, IMapper mapper)
    {
        _productPriceRepository = productPriceRepository;
        _mapper = mapper;
    }

    public async Task<UpdateProductPriceResult> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductPriceCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var price = await _productPriceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (price == null)
            throw new KeyNotFoundException($"Price with ID {request.Id} not found");

        var hasOverlap = await _productPriceRepository.HasOverlappingPriceAsync(
            price.ProductId,
            request.PriceType,
            request.EffectiveStartAt,
            request.EffectiveEndAt,
            request.Id,
            cancellationToken);

        if (hasOverlap)
            throw new InvalidOperationException("There is already a price for this product and price type in the informed period.");

        price.Update(
            request.PriceType,
            request.Price,
            request.EffectiveStartAt,
            request.EffectiveEndAt);

        try
        {
            await _productPriceRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("EX_ProductPrices_NoOverlap", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException("There is already a price for this product and price type in the informed period.");
        }

        return _mapper.Map<UpdateProductPriceResult>(price);
    }
}
