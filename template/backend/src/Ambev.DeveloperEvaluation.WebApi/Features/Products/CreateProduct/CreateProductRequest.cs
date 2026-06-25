using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductRequest
{
    public string Description { get; set; } = string.Empty;
    public string UnitMeasure { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }
    public decimal MaxSaleQuantity { get; set; }
    public List<string> Barcodes { get; set; } = [];
}
