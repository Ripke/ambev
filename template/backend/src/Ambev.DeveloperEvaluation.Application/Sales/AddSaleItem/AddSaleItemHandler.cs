using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;

public class AddSaleItemHandler : IRequestHandler<AddSaleItemCommand, AddSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IMapper _mapper;

    public AddSaleItemHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IProductPriceRepository productPriceRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _productPriceRepository = productPriceRepository;
        _mapper = mapper;
    }

    public async Task<AddSaleItemResult> Handle(AddSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new AddSaleItemCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        EnsureSaleIsOpen(sale);

        var product = await ResolveProductAsync(request, cancellationToken);
        var price = await _productPriceRepository.GetCurrentPriceByProductIdAsync(
            product.Id,
            PriceType.Cash,
            DateTime.UtcNow,
            cancellationToken);

        if (price == null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.ProductId), $"No current cash price found for product {product.Id}.")
            });
        }

        ValidateMaxSaleQuantity(sale, product.Id, request.Quantity, null, product.MaxSaleQuantity);

        var ean = request.Ean?.Trim() ?? product.Barcodes.FirstOrDefault()?.Barcode;
        if (string.IsNullOrWhiteSpace(ean))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.Ean), "Product barcode is required.") });

        sale.AddItem(ean, product.Id, product.Description, request.Quantity, price.Price);

        await _saleRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AddSaleItemResult>(sale);
    }

    private async Task<Product> ResolveProductAsync(AddSaleItemCommand request, CancellationToken cancellationToken)
    {
        if (request.ProductId.HasValue)
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId.Value, cancellationToken);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {request.ProductId.Value} not found");

            return product;
        }

        var byBarcode = await _productRepository.GetByBarcodeAsync(request.Ean!.Trim(), cancellationToken);
        if (byBarcode == null)
            throw new KeyNotFoundException($"Product with barcode {request.Ean} not found");

        return byBarcode;
    }

    private static void EnsureSaleIsOpen(Sale sale)
    {
        if (sale.Status == SaleStatus.Open)
            return;

        throw new ValidationException(new[]
        {
            new ValidationFailure(nameof(sale.Status), "Items can only be changed while the sale is open.")
        });
    }

    private static void ValidateMaxSaleQuantity(Sale sale, Guid productId, decimal newQuantity, Guid? currentItemId, decimal maxSaleQuantity)
    {
        var quantityInSale = sale.Items
            .Where(item => !item.IsCanceled && item.ProductId == productId && item.Id != currentItemId)
            .Sum(item => item.Quantity);

        if (quantityInSale + newQuantity <= maxSaleQuantity)
            return;

        throw new ValidationException(new[]
        {
            new ValidationFailure(nameof(productId), "Product quantity exceeds MaxSaleQuantity for this sale.")
        });
    }
}
