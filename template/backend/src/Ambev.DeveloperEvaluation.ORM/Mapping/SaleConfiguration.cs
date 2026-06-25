using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(sale => sale.Id);
        builder.Property(sale => sale.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(sale => sale.SaleNumber)
            .HasColumnType("bigint")
            .HasDefaultValueSql("nextval('sales_numbers')")
            .ValueGeneratedOnAdd();

        builder.HasIndex(sale => sale.SaleNumber)
            .IsUnique();

        builder.Property(sale => sale.Version)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(sale => sale.StartedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(sale => sale.MovementDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(sale => sale.FinishedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(sale => sale.Subtotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(sale => sale.Total)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(sale => sale.AdditionalAmountTotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(sale => sale.DiscountAmountTotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(sale => sale.PaymentAmountTotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(sale => sale.ChangeAmountTotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(sale => sale.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(sale => sale.StatusChangedDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(sale => sale.IsCanceled)
            .IsRequired();

        builder.Property(sale => sale.CancellationAuthorizerId)
            .HasColumnType("uuid");

        builder.Property(sale => sale.CancellationAuthorizerName)
            .HasMaxLength(200);

        builder.Property(sale => sale.CancellationReason)
            .HasMaxLength(500);

        builder.Property(sale => sale.CompanyId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(sale => sale.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(sale => sale.CustomerId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(sale => sale.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(sale => sale.CustomerId);
        builder.HasIndex(sale => sale.CompanyId);
        builder.HasIndex(sale => sale.Status);
    }
}
