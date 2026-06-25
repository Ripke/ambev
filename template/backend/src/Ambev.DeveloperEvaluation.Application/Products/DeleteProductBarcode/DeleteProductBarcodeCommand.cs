using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProductBarcode;

public record DeleteProductBarcodeCommand(Guid ProductId, Guid BarcodeId) : IRequest<DeleteProductBarcodeResponse>;
