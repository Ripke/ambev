using Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountHandler : IRequestHandler<ApplyManualSaleItemDiscountCommand, ApplyManualSaleItemDiscountResult>
{
    private readonly IServiceAdditionDiscount _adjustmentService;
    private readonly IMapper _mapper;

    public ApplyManualSaleItemDiscountHandler(IServiceAdditionDiscount adjustmentService, IMapper mapper)
    {
        _adjustmentService = adjustmentService;
        _mapper = mapper;
    }

    public async Task<ApplyManualSaleItemDiscountResult> Handle(ApplyManualSaleItemDiscountCommand request, CancellationToken cancellationToken)
    {
        var validator = new ApplyManualSaleItemDiscountCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _adjustmentService.Apply(
            Domain.Enums.SaleItemAdjustmentKind.Discount,
            Domain.Enums.SaleItemAdjustmentType.Manual,
            request.SaleId,
            request.ItemId,
            request.Amount,
            request.AuthorizerId,
            request.Reason,
            cancellationToken);

        return _mapper.Map<ApplyManualSaleItemDiscountResult>(sale);
    }
}
