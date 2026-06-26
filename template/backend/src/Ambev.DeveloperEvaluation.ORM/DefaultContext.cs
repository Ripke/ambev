using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM;

public class DefaultContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<SalePayment> SalesPayments { get; set; }
    public DbSet<SaleChange> SalesChanges { get; set; }
    public DbSet<SalesItemDiscount> SalesItemDiscounts { get; set; }
    public DbSet<SalesItemAddition> SalesItemAdditions { get; set; }
    public DbSet<SalesPromotion> SalesPromotions { get; set; }
    public DbSet<SalesPromotionItem> SalesPromotionItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductBarcode> ProductBarcodes { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<long>("sales_numbers")
            .StartsAt(1)
            .IncrementsBy(1);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplySnakeCaseNamingConvention(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void ApplySnakeCaseNamingConvention(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!entityType.IsOwned() && entityType.ClrType is not null)
            {
                entityType.SetTableName(ToSnakeCase(Pluralize(entityType.ClrType.Name)));
            }

            foreach (var property in entityType.GetProperties())
            {
                if (entityType.IsOwned() && property.IsPrimaryKey())
                {
                    property.SetColumnName("id");
                    continue;
                }

                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string Pluralize(string name)
    {
        if (name.EndsWith("y", true, CultureInfo.InvariantCulture))
        {
            return $"{name[..^1]}ies";
        }

        return $"{name}s";
    }

    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        var characters = new List<char>(name.Length + 8);

        for (var index = 0; index < name.Length; index++)
        {
            var character = name[index];

            if (char.IsUpper(character))
            {
                var hasPreviousCharacter = index > 0;
                var hasNextCharacter = index + 1 < name.Length;
                var previousCharacterIsLowerOrDigit = hasPreviousCharacter &&
                    (char.IsLower(name[index - 1]) || char.IsDigit(name[index - 1]));
                var nextCharacterIsLower = hasNextCharacter && char.IsLower(name[index + 1]);

                if (hasPreviousCharacter && (previousCharacterIsLowerOrDigit || nextCharacterIsLower))
                {
                    characters.Add('_');
                }

                characters.Add(char.ToLowerInvariant(character));
                continue;
            }

            characters.Add(char.ToLowerInvariant(character));
        }

        return new string(characters.ToArray());
    }
}

public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(
               connectionString,
               b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
        ).EnableSensitiveDataLogging()
                 .LogTo(Console.WriteLine, LogLevel.Information);

        return new DefaultContext(builder.Options);
    }
}
