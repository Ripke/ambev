using Ambev.DeveloperEvaluation.Application.Prices.CreateProductPrice;
using Ambev.DeveloperEvaluation.Application.Prices.DeleteProductPrice;
using Ambev.DeveloperEvaluation.Application.Prices.GetProductPrice;
using Ambev.DeveloperEvaluation.Application.Prices.ListProductPrices;
using Ambev.DeveloperEvaluation.Application.Prices.UpdateProductPrice;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Prices.CreateProductPrice;
using Ambev.DeveloperEvaluation.WebApi.Features.Prices.DeleteProductPrice;
using Ambev.DeveloperEvaluation.WebApi.Features.Prices.GetProductPrice;
using Ambev.DeveloperEvaluation.WebApi.Features.Prices.ListProductPrices;
using Ambev.DeveloperEvaluation.WebApi.Features.Prices.UpdateProductPrice;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Prices;

[ApiController]
[Route("api")]
public class PricesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public PricesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("products/{productId}/prices")]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateProductPriceResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePrice([FromRoute] Guid productId, [FromBody] CreateProductPriceRequest request, CancellationToken cancellationToken)
    {
        request.ProductId = productId;
        var validator = new CreateProductPriceRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateProductPriceCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateProductPriceResponse>
        {
            Success = true,
            Message = "Price created successfully",
            Data = _mapper.Map<CreateProductPriceResponse>(response)
        });
    }

    [HttpGet("products/{productId}/prices")]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<ListProductPricesResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListPrices([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        var request = new ListProductPricesRequest { ProductId = productId };
        var validator = new ListProductPricesRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var response = await _mediator.Send(new ListProductPricesCommand(productId), cancellationToken);

        return Ok(new ApiResponseWithData<IEnumerable<ListProductPricesResponse>>
        {
            Success = true,
            Message = "Prices retrieved successfully",
            Data = _mapper.Map<IEnumerable<ListProductPricesResponse>>(response)
        });
    }

    [HttpGet("prices/{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductPriceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPrice([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetProductPriceRequest { Id = id };
        var validator = new GetProductPriceRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetProductPriceCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetProductPriceResponse>
        {
            Success = true,
            Message = "Price retrieved successfully",
            Data = _mapper.Map<GetProductPriceResponse>(response)
        });
    }

    [HttpPut("prices/{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateProductPriceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePrice([FromRoute] Guid id, [FromBody] UpdateProductPriceRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new UpdateProductPriceRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateProductPriceCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateProductPriceResponse>
        {
            Success = true,
            Message = "Price updated successfully",
            Data = _mapper.Map<UpdateProductPriceResponse>(response)
        });
    }

    [HttpDelete("prices/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePrice([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteProductPriceRequest { Id = id };
        var validator = new DeleteProductPriceRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteProductPriceCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Price deleted successfully"
        });
    }
}
