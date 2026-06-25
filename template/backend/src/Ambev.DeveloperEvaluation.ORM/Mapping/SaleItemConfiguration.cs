using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SalesItems");

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(item => item.IdSales)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(item => item.SequentialNumber)
            .IsRequired();

        builder.Property(item => item.ProductEan)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(item => item.ProductId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(item => item.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(item => item.Subtotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(item => item.AdditionalAmountTotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(item => item.DiscountAmountTotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(item => item.Total)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(item => item.IsCanceled)
            .IsRequired();

        builder.Property(item => item.CancellationAuthorizerId)
            .HasColumnType("uuid");

        builder.Property(item => item.CancellationAuthorizerName)
            .HasMaxLength(200);

        builder.Property(item => item.CancellationReason)
            .HasMaxLength(500);

        builder.Property(item => item.SaleDateTime)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(item => item.IdSales);
        builder.HasIndex(item => new { item.IdSales, item.SequentialNumber }).IsUnique();

        builder.HasOne(item => item.Sale)
            .WithMany(sale => sale.Items)
            .HasForeignKey(item => item.IdSales)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
