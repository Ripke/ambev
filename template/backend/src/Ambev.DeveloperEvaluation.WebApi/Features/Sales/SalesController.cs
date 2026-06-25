using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByCustomer;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ReopenSale;
using Ambev.DeveloperEvaluation.Application.Sales.SubtotalizeSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetCurrentSaleByCustomer;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ReopenSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.SubtotalizeSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = _mapper.Map<CreateSaleResponse>(response)
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetSaleCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetSaleResponse>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = _mapper.Map<GetSaleResponse>(response)
        });
    }

    [HttpGet("customers/{customerId}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetCurrentSaleByCustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentSaleByCustomer([FromRoute] Guid customerId, CancellationToken cancellationToken)
    {
        var request = new GetCurrentSaleByCustomerRequest { CustomerId = customerId };
        var validator = new GetCurrentSaleByCustomerRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetCurrentSaleByCustomerCommand>(request.CustomerId);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetCurrentSaleByCustomerResponse>
        {
            Success = true,
            Message = "Current sale retrieved successfully",
            Data = _mapper.Map<GetCurrentSaleByCustomerResponse>(response)
        });
    }

    [HttpPost("{id}/subtotalize")]
    [ProducesResponseType(typeof(ApiResponseWithData<SubtotalizeSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubtotalizeSale([FromRoute] Guid id, [FromBody] SubtotalizeSaleRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new SubtotalizeSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<SubtotalizeSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<SubtotalizeSaleResponse>
        {
            Success = true,
            Message = "Sale subtotalized successfully",
            Data = _mapper.Map<SubtotalizeSaleResponse>(response)
        });
    }

    [HttpPost("{id}/reopen")]
    [ProducesResponseType(typeof(ApiResponseWithData<ReopenSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReopenSale([FromRoute] Guid id, [FromBody] ReopenSaleRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new ReopenSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<ReopenSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<ReopenSaleResponse>
        {
            Success = true,
            Message = "Sale reopened successfully",
            Data = _mapper.Map<ReopenSaleResponse>(response)
        });
    }

    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, [FromBody] CancelSaleRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CancelSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CancelSaleResponse>
        {
            Success = true,
            Message = "Sale canceled successfully",
            Data = _mapper.Map<CancelSaleResponse>(response)
        });
    }
}
