using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalePaymentConfiguration : IEntityTypeConfiguration<SalePayment>
{
    public void Configure(EntityTypeBuilder<SalePayment> builder)
    {
        builder.ToTable("sale_payments");

        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(payment => payment.SaleId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(payment => payment.PaymentType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(payment => payment.Value)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(payment => payment.PaidAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(payment => payment.SaleId);
        builder.HasIndex(payment => new { payment.SaleId, payment.PaidAt });

        builder.HasOne(payment => payment.Sale)
            .WithMany(sale => sale.Payments)
            .HasForeignKey(payment => payment.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
