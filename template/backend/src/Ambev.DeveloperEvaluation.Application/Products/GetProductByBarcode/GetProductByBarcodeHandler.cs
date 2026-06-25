using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductByBarcode;

public class GetProductByBarcodeHandler : IRequestHandler<GetProductByBarcodeCommand, GetProductByBarcodeResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByBarcodeHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetProductByBarcodeResult> Handle(GetProductByBarcodeCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetProductByBarcodeValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = await _productRepository.GetByBarcodeAsync(request.Barcode.Trim(), cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with barcode {request.Barcode} not found");

        return _mapper.Map<GetProductByBarcodeResult>(product);
    }
}
