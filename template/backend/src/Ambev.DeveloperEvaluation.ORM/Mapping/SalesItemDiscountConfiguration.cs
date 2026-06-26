using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesItemDiscountConfiguration : IEntityTypeConfiguration<SalesItemDiscount>
{
    public void Configure(EntityTypeBuilder<SalesItemDiscount> builder)
    {
        builder.ToTable("sales_item_discounts");

        builder.HasKey(discount => discount.Id);
        builder.Property(discount => discount.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(discount => discount.SaleItemId)
            .HasColumnName("sale_item_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(discount => discount.AdjustmentType)
            .HasColumnName("adjustment_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(discount => discount.Amount)
            .HasColumnName("amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(discount => discount.AuthorizerId)
            .HasColumnName("authorizer_id")
            .HasColumnType("uuid");

        builder.Property(discount => discount.AuthorizerName)
            .HasColumnName("authorizer_name")
            .HasMaxLength(200);

        builder.Property(discount => discount.Reason)
            .HasColumnName("reason")
            .HasMaxLength(500);

        builder.Property(discount => discount.OccurredAt)
            .HasColumnName("occurred_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(discount => discount.SaleItemId);

        builder.HasOne(discount => discount.SaleItem)
            .WithMany(item => item.Discounts)
            .HasForeignKey(discount => discount.SaleItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
