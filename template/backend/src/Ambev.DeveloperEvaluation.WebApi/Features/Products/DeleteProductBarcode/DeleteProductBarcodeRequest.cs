namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProductBarcode;

public class DeleteProductBarcodeRequest
{
    public Guid ProductId { get; set; }
    public Guid BarcodeId { get; set; }
}
