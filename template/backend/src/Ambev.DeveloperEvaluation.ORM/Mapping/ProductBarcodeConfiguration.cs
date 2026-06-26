using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductBarcodeConfiguration : IEntityTypeConfiguration<ProductBarcode>
{
    public void Configure(EntityTypeBuilder<ProductBarcode> builder)
    {
        builder.ToTable("product_barcodes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.ProductId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.Barcode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.Barcode)
            .IsUnique();

        builder.HasIndex(x => new { x.ProductId, x.Barcode })
            .IsUnique();
    }
}
