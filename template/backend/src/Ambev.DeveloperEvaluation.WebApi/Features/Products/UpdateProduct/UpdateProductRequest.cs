using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductRequest
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string UnitMeasure { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }
    public decimal MaxSaleQuantity { get; set; }
}
