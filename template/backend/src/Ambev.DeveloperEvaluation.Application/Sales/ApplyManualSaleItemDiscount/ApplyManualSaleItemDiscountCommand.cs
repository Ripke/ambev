using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ApplyManualSaleItemDiscount;

public class ApplyManualSaleItemDiscountCommand : IRequest<ApplyManualSaleItemDiscountResult>
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Valor { get; set; }
    public Guid AutorizadorId { get; set; }
    public string? Motivo { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new ApplyManualSaleItemDiscountCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }
}
