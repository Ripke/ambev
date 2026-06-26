using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesItemAdditionConfiguration : IEntityTypeConfiguration<SalesItemAddition>
{
    public void Configure(EntityTypeBuilder<SalesItemAddition> builder)
    {
        builder.ToTable("SALES_ITEMS_additions");

        builder.HasKey(addition => addition.Id);
        builder.Property(addition => addition.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(addition => addition.IdSalesItem)
            .HasColumnName("id_sales_item")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(addition => addition.Tipo)
            .HasColumnName("tipo")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(addition => addition.Valor)
            .HasColumnName("valor")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(addition => addition.AutorizadorId)
            .HasColumnName("autorizador_id")
            .HasColumnType("uuid");

        builder.Property(addition => addition.AutorizadorName)
            .HasColumnName("autorizador_name")
            .HasMaxLength(200);

        builder.Property(addition => addition.Motivo)
            .HasColumnName("motivo")
            .HasMaxLength(500);

        builder.Property(addition => addition.DataHora)
            .HasColumnName("datahora")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(addition => addition.IdSalesItem);

        builder.HasOne(addition => addition.SaleItem)
            .WithMany(item => item.Additions)
            .HasForeignKey(addition => addition.IdSalesItem)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
