using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(company => company.Id);
        builder.Property(company => company.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(company => company.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(company => company.Cnpj)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(company => company.Cnpj)
            .IsUnique();

        builder.OwnsOne(company => company.Address, address =>
        {
            address.Property(value => value.Street)
                .HasColumnName("street")
                .IsRequired()
                .HasMaxLength(200);

            address.Property(value => value.Number)
                .HasColumnName("number")
                .IsRequired()
                .HasMaxLength(20);

            address.Property(value => value.Complement)
                .HasColumnName("complement")
                .HasMaxLength(100);

            address.Property(value => value.District)
                .HasColumnName("district")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(value => value.City)
                .HasColumnName("city")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(value => value.State)
                .HasColumnName("state")
                .IsRequired()
                .HasMaxLength(2);

            address.Property(value => value.ZipCode)
                .HasColumnName("zip_code")
                .IsRequired()
                .HasMaxLength(8);

            address.Property(value => value.Country)
                .HasColumnName("country")
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.Property(company => company.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(company => company.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(company => company.UpdatedAt)
            .HasColumnType("timestamp with time zone");
    }
}
