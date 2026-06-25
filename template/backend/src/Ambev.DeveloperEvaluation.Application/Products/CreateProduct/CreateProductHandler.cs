using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var normalizedBarcodes = command.Barcodes
            .Select(x => x.Trim())
            .ToList();

        foreach (var barcode in normalizedBarcodes)
        {
            if (await _productRepository.BarcodeExistsAsync(barcode, cancellationToken: cancellationToken))
                throw new InvalidOperationException($"Barcode {barcode} already exists.");
        }

        var product = Domain.Entities.Product.Create(
            command.Description,
            command.UnitMeasure,
            command.Brand,
            command.Model,
            command.ProductType,
            command.MaxSaleQuantity,
            normalizedBarcodes);

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);
        return _mapper.Map<CreateProductResult>(createdProduct);
    }
}
