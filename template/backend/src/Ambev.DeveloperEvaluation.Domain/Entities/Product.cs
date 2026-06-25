using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Product : BaseEntity
{
    public string Description { get; private set; } = string.Empty;
    public string UnitMeasure { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public ProductType ProductType { get; private set; }
    public decimal MaxSaleQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public List<ProductBarcode> Barcodes { get; private set; } = [];
    public List<ProductPrice> Prices { get; private set; } = [];

    public Product()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static Product Create(
        string description,
        string unitMeasure,
        string brand,
        string model,
        ProductType productType,
        decimal maxSaleQuantity,
        IEnumerable<string> barcodes)
    {
        var product = new Product();
        product.SetDetails(description, unitMeasure, brand, model, productType, maxSaleQuantity);
        product.CreatedAt = DateTime.UtcNow;
        product.AddEvent(new ProductCreatedEvent(product.Id));

        foreach (var barcode in barcodes)
        {
            product.AssociateBarcode(barcode);
        }

        return product;
    }

    public void Update(
        string description,
        string unitMeasure,
        string brand,
        string model,
        ProductType productType,
        decimal maxSaleQuantity)
    {
        SetDetails(description, unitMeasure, brand, model, productType, maxSaleQuantity);
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new ProductUpdatedEvent(Id, UpdatedAt.Value));
    }

    public ProductBarcode AssociateBarcode(string barcode)
    {
        var normalizedBarcode = barcode.Trim();
        var productBarcode = ProductBarcode.Create(Id, normalizedBarcode);
        Barcodes.Add(productBarcode);
        AddEvent(new ProductBarcodeAssociatedEvent(Id, normalizedBarcode, productBarcode.CreatedAt));
        return productBarcode;
    }

    public void RemoveBarcode(ProductBarcode barcode)
    {
        if (Barcodes.Remove(barcode))
        {
            barcode.MarkAsRemoved();
        }
    }

    public void MarkAsDeleted()
    {
        AddEvent(new ProductDeletedEvent(Id, DateTime.UtcNow));
    }

    private void SetDetails(
        string description,
        string unitMeasure,
        string brand,
        string model,
        ProductType productType,
        decimal maxSaleQuantity)
    {
        Description = description.Trim();
        UnitMeasure = unitMeasure.Trim();
        Brand = brand.Trim();
        Model = model.Trim();
        ProductType = productType;
        MaxSaleQuantity = maxSaleQuantity;
    }
}
