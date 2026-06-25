using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(customer => customer.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(customer => customer.BirthDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(customer => customer.EncryptedCpf)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(customer => customer.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(customer => customer.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(customer => customer.UpdatedAt)
            .HasColumnType("timestamp with time zone");
    }
}
