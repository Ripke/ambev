using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItemQuantity;

public class UpdateSaleItemQuantityHandler : IRequestHandler<UpdateSaleItemQuantityCommand, UpdateSaleItemQuantityResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateSaleItemQuantityHandler(ISaleRepository saleRepository, IProductRepository productRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<UpdateSaleItemQuantityResult> Handle(UpdateSaleItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleItemQuantityCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        EnsureSaleIsOpen(sale);

        var item = sale.GetItemOrThrow(request.ItemId);
        if (item.IsCanceled)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.ItemId), "Canceled item cannot be changed.")
            });
        }

        var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");

        ValidateMaxSaleQuantity(sale, item.ProductId, request.Quantity, item.Id, product.MaxSaleQuantity);

        sale.UpdateItemQuantity(item.Id, request.Quantity);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UpdateSaleItemQuantityResult>(sale);
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

    private static void ValidateMaxSaleQuantity(Sale sale, Guid productId, decimal newQuantity, Guid currentItemId, decimal maxSaleQuantity)
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
