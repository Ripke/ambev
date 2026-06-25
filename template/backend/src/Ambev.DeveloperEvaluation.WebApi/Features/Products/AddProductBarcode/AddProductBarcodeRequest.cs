namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.AddProductBarcode;

public class AddProductBarcodeRequest
{
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = string.Empty;
}
