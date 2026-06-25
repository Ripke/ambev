using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.AddProductBarcode;

public class AddProductBarcodeHandler : IRequestHandler<AddProductBarcodeCommand, AddProductBarcodeResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public AddProductBarcodeHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<AddProductBarcodeResult> Handle(AddProductBarcodeCommand request, CancellationToken cancellationToken)
    {
        var validator = new AddProductBarcodeCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");

        var normalizedBarcode = request.Barcode.Trim();
        if (await _productRepository.BarcodeExistsAsync(normalizedBarcode, cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Barcode {normalizedBarcode} already exists.");

        var barcode = product.AssociateBarcode(normalizedBarcode);
        await _productRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AddProductBarcodeResult>(barcode);
    }
}
