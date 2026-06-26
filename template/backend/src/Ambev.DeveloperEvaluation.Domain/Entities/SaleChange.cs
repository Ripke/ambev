using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleChange : BaseEntity
{
    
    public Guid IdSales { get; private set; }
    public PaymentType TypePayment { get; private set; }
    public decimal Value { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Sale Sale { get; private set; } = null!;

    public SaleChange()
    {
    }

    public static SaleChange Create(Guid idSales, PaymentType typePayment, decimal value)
    {
        if (idSales == Guid.Empty)
            throw new ArgumentException("Sale ID is required.", nameof(idSales));

        if (typePayment != PaymentType.Cash)
            throw new ArgumentException("Only cash payments can generate change.", nameof(typePayment));

        if (value <= 0)
            throw new ArgumentException("Change value must be greater than zero.", nameof(value));

        return new SaleChange
        {
            IdSales = idSales,
            TypePayment = typePayment,
            Value = decimal.Round(value, 2, MidpointRounding.AwayFromZero),
            ChangedAt = DateTime.UtcNow
        };
    }
}
