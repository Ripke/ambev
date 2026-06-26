using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesItemDiscountConfiguration : IEntityTypeConfiguration<SalesItemDiscount>
{
    public void Configure(EntityTypeBuilder<SalesItemDiscount> builder)
    {
        builder.ToTable("SALES_ITEMS_discount");

        builder.HasKey(discount => discount.Id);
        builder.Property(discount => discount.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(discount => discount.IdSalesItem)
            .HasColumnName("id_sales_item")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(discount => discount.TipoDesconto)
            .HasColumnName("tipo_desconto")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(discount => discount.Valor)
            .HasColumnName("valor")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(discount => discount.AutorizadorId)
            .HasColumnName("autorizador_id")
            .HasColumnType("uuid");

        builder.Property(discount => discount.AutorizadorName)
            .HasColumnName("autorizador_name")
            .HasMaxLength(200);

        builder.Property(discount => discount.Motivo)
            .HasColumnName("motivo")
            .HasMaxLength(500);

        builder.Property(discount => discount.DataHora)
            .HasColumnName("datahora")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(discount => discount.IdSalesItem);

        builder.HasOne(discount => discount.SaleItem)
            .WithMany(item => item.Discounts)
            .HasForeignKey(discount => discount.IdSalesItem)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
