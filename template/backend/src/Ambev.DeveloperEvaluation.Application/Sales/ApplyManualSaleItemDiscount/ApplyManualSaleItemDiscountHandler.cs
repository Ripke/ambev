using Ambev.DeveloperEvaluation.Application.Sales.AdditionDiscount;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountHandler : IRequestHandler<ApplyManualSaleItemDiscountCommand, ApplyManualSaleItemDiscountResult>
{
    private readonly IServiceAdditionDiscount _serviceAcrescimoDesconto;
    private readonly IMapper _mapper;

    public ApplyManualSaleItemDiscountHandler(IServiceAdditionDiscount serviceAcrescimoDesconto, IMapper mapper)
    {
        _serviceAcrescimoDesconto = serviceAcrescimoDesconto;
        _mapper = mapper;
    }

    public async Task<ApplyManualSaleItemDiscountResult> Handle(ApplyManualSaleItemDiscountCommand request, CancellationToken cancellationToken)
    {
        var validator = new ApplyManualSaleItemDiscountCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _serviceAcrescimoDesconto.Apply(
            Domain.Enums.AdditionDiscount.Desconto,
            Domain.Enums.AdditionDiscountTypes.Manual,
            request.SaleId,
            request.ItemId,
            request.Valor,
            request.AutorizadorId,
            request.Motivo,
            cancellationToken);

        return _mapper.Map<ApplyManualSaleItemDiscountResult>(sale);
    }
}
