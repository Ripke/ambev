using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesPromotionItemConfiguration : IEntityTypeConfiguration<SalesPromotionItem>
{
    public void Configure(EntityTypeBuilder<SalesPromotionItem> builder)
    {
        builder.ToTable("sales_promotion_item");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.PromotionId)
            .HasColumnName("promotion_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.MinimumQuantity)
            .HasColumnName("minimum_quantity")
            .IsRequired();

        builder.Property(x => x.MaximumQuantity)
            .HasColumnName("maximum_quantity")
            .IsRequired();

        builder.Property(x => x.DiscountType)
            .HasColumnName("discount_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DiscountValue)
            .HasColumnName("discount_value")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(x => x.PromotionId);
    }
}
