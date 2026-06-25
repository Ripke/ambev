using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductByBarcode;

public record GetProductByBarcodeCommand(string Barcode) : IRequest<GetProductByBarcodeResult>;
