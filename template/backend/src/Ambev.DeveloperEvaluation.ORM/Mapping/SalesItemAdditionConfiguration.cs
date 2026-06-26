using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesItemAdditionConfiguration : IEntityTypeConfiguration<SalesItemAddition>
{
    public void Configure(EntityTypeBuilder<SalesItemAddition> builder)
    {
        builder.ToTable("sales_item_additions");

        builder.HasKey(addition => addition.Id);
        builder.Property(addition => addition.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(addition => addition.SaleItemId)
            .HasColumnName("sale_item_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(addition => addition.AdjustmentType)
            .HasColumnName("adjustment_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(addition => addition.Amount)
            .HasColumnName("amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(addition => addition.AuthorizerId)
            .HasColumnName("authorizer_id")
            .HasColumnType("uuid");

        builder.Property(addition => addition.AuthorizerName)
            .HasColumnName("authorizer_name")
            .HasMaxLength(200);

        builder.Property(addition => addition.Reason)
            .HasColumnName("reason")
            .HasMaxLength(500);

        builder.Property(addition => addition.OccurredAt)
            .HasColumnName("occurred_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(addition => addition.SaleItemId);

        builder.HasOne(addition => addition.SaleItem)
            .WithMany(item => item.Additions)
            .HasForeignKey(addition => addition.SaleItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
