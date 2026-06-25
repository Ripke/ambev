namespace Ambev.DeveloperEvaluation.Application.Products.AddProductBarcode;

public class AddProductBarcodeResult
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
