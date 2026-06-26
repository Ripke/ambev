using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SalePayment : BaseEntity
{
    public Guid IdSales { get; private set; }
    public PaymentType TypePayment { get; private set; }
    public decimal Value { get; private set; }
    public DateTime PaidAt { get; private set; }
    public Sale Sale { get; private set; } = null!;

    public SalePayment()
    {
    }

    public static SalePayment Create(Guid idSales, PaymentType typePayment, decimal value)
    {
        if (idSales == Guid.Empty)
            throw new ArgumentException("Sale ID is required.", nameof(idSales));

        if (typePayment == PaymentType.Unknown)
            throw new ArgumentException("Payment type is required.", nameof(typePayment));

        if (value <= 0)
            throw new ArgumentException("Payment value must be greater than zero.", nameof(value));

        return new SalePayment
        {
            IdSales = idSales,
            TypePayment = typePayment,
            Value = decimal.Round(value, 2, MidpointRounding.AwayFromZero),
            PaidAt = DateTime.UtcNow
        };
    }
}
