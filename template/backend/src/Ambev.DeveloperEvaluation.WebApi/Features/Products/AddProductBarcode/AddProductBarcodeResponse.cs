namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.AddProductBarcode;

public class AddProductBarcodeResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
