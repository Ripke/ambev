using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
{
    public void Configure(EntityTypeBuilder<ProductPrice> builder)
    {
        builder.ToTable("product_prices");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.ProductId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.PriceType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.EffectiveStartAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.EffectiveEndAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_ProductPrices_EffectiveRange",
            "\"EffectiveEndAt\" > \"EffectiveStartAt\""));
    }
}
