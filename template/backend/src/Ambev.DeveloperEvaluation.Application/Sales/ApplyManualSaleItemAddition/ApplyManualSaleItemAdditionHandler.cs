using Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionHandler : IRequestHandler<ApplyManualSaleItemAdditionCommand, ApplyManualSaleItemAdditionResult>
{
    private readonly IServiceAdditionDiscount _adjustmentService;
    private readonly IMapper _mapper;

    public ApplyManualSaleItemAdditionHandler(IServiceAdditionDiscount adjustmentService, IMapper mapper)
    {
        _adjustmentService = adjustmentService;
        _mapper = mapper;
    }

    public async Task<ApplyManualSaleItemAdditionResult> Handle(ApplyManualSaleItemAdditionCommand request, CancellationToken cancellationToken)
    {
        var validator = new ApplyManualSaleItemAdditionCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _adjustmentService.Apply(
            Domain.Enums.SaleItemAdjustmentKind.Addition,
            Domain.Enums.SaleItemAdjustmentType.Manual,
            request.SaleId,
            request.ItemId,
            request.Amount,
            request.AuthorizerId,
            request.Reason,
            cancellationToken);

        return _mapper.Map<ApplyManualSaleItemAdditionResult>(sale);
    }
}
