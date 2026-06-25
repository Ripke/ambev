using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductResult
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string UnitMeasure { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }
    public decimal MaxSaleQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
