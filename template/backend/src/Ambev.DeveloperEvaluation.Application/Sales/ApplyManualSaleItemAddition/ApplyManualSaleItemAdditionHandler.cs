using Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemAddition;

public class ApplyManualSaleItemAdditionHandler : IRequestHandler<ApplyManualSaleItemAdditionCommand, ApplyManualSaleItemAdditionResult>
{
    private readonly IServiceAdditionDiscount _serviceAcrescimoDesconto;
    private readonly IMapper _mapper;

    public ApplyManualSaleItemAdditionHandler(IServiceAdditionDiscount serviceAcrescimoDesconto, IMapper mapper)
    {
        _serviceAcrescimoDesconto = serviceAcrescimoDesconto;
        _mapper = mapper;
    }

    public async Task<ApplyManualSaleItemAdditionResult> Handle(ApplyManualSaleItemAdditionCommand request, CancellationToken cancellationToken)
    {
        var validator = new ApplyManualSaleItemAdditionCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _serviceAcrescimoDesconto.Apply(
            Domain.Enums.AdditionDiscount.Acrescimo,
            Domain.Enums.AdditionDiscountTypes.Manual,
            request.SaleId,
            request.ItemId,
            request.Valor,
            request.AutorizadorId,
            request.Motivo,
            cancellationToken);

        return _mapper.Map<ApplyManualSaleItemAdditionResult>(sale);
    }
}
