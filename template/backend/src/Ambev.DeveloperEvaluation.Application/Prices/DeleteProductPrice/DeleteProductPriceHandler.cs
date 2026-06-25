using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Prices.DeleteProductPrice;

public class DeleteProductPriceHandler : IRequestHandler<DeleteProductPriceCommand, DeleteProductPriceResponse>
{
    private readonly IProductPriceRepository _productPriceRepository;

    public DeleteProductPriceHandler(IProductPriceRepository productPriceRepository)
    {
        _productPriceRepository = productPriceRepository;
    }

    public async Task<DeleteProductPriceResponse> Handle(DeleteProductPriceCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteProductPriceValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var price = await _productPriceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (price == null)
            throw new KeyNotFoundException($"Price with ID {request.Id} not found");

        price.MarkAsRemoved();
        await _productPriceRepository.DeleteAsync(price, cancellationToken);

        return new DeleteProductPriceResponse { Success = true };
    }
}
