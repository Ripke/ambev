using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleChangeConfiguration : IEntityTypeConfiguration<SaleChange>
{
    public void Configure(EntityTypeBuilder<SaleChange> builder)
    {
        builder.ToTable("sale_changes");

        builder.HasKey(change => change.Id);
        builder.Property(change => change.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(change => change.SaleId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(change => change.PaymentType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(change => change.Value)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(change => change.ChangedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(change => change.SaleId);
        builder.HasIndex(change => new { change.SaleId, change.ChangedAt });

        builder.HasOne(change => change.Sale)
            .WithMany(sale => sale.Changes)
            .HasForeignKey(change => change.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
