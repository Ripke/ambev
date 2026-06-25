namespace Ambev.DeveloperEvaluation.Application.Products;

public class ProductBarcodeResult
{
    public Guid Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
