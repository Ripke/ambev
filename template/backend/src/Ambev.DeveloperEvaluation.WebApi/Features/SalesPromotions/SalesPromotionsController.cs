using Ambev.DeveloperEvaluation.Application.SalesPromotions.CreateSalesPromotion;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.DeleteSalesPromotion;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.GetSalesPromotion;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.ListSalesPromotions;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.UpdateSalesPromotion;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.CreateSalesPromotion;
using Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.DeleteSalesPromotion;
using Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.GetSalesPromotion;
using Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.ListSalesPromotions;
using Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.UpdateSalesPromotion;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{nameof(UserRole.Manager)},{nameof(UserRole.Admin)}")]
public class SalesPromotionsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesPromotionsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSalesPromotionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSalesPromotionRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSalesPromotionRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSalesPromotionCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateSalesPromotionResponse>
        {
            Success = true,
            Message = "Sales promotion created successfully",
            Data = _mapper.Map<CreateSalesPromotionResponse>(response)
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSalesPromotionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSalesPromotionRequest { Id = id };
        var validator = new GetSalesPromotionRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetSalesPromotionCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetSalesPromotionResponse>
        {
            Success = true,
            Message = "Sales promotion retrieved successfully",
            Data = _mapper.Map<GetSalesPromotionResponse>(response)
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<ListSalesPromotionsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListSalesPromotionsCommand(), cancellationToken);

        return Ok(new ApiResponseWithData<IEnumerable<ListSalesPromotionsResponse>>
        {
            Success = true,
            Message = "Sales promotions retrieved successfully",
            Data = _mapper.Map<IEnumerable<ListSalesPromotionsResponse>>(response)
        });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSalesPromotionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSalesPromotionRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new UpdateSalesPromotionRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSalesPromotionCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateSalesPromotionResponse>
        {
            Success = true,
            Message = "Sales promotion updated successfully",
            Data = _mapper.Map<UpdateSalesPromotionResponse>(response)
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteSalesPromotionRequest { Id = id };
        var validator = new DeleteSalesPromotionRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteSalesPromotionCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Sales promotion deleted successfully"
        });
    }
}
