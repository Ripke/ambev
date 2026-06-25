using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProductBarcode;

public class DeleteProductBarcodeHandler : IRequestHandler<DeleteProductBarcodeCommand, DeleteProductBarcodeResponse>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductBarcodeHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<DeleteProductBarcodeResponse> Handle(DeleteProductBarcodeCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteProductBarcodeValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var barcode = await _productRepository.GetBarcodeByIdAsync(request.ProductId, request.BarcodeId, cancellationToken);
        if (barcode == null)
            throw new KeyNotFoundException($"Barcode with ID {request.BarcodeId} not found for product {request.ProductId}");

        barcode.MarkAsRemoved();
        await _productRepository.DeleteBarcodeAsync(barcode, cancellationToken);
        return new DeleteProductBarcodeResponse { Success = true };
    }
}
