using Ambev.DeveloperEvaluation.Application.Companies.CreateCompany;
using Ambev.DeveloperEvaluation.Application.Companies.DeleteCompany;
using Ambev.DeveloperEvaluation.Application.Companies.GetCompany;
using Ambev.DeveloperEvaluation.Application.Companies.ListCompanies;
using Ambev.DeveloperEvaluation.Application.Companies.UpdateCompany;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Companies.CreateCompany;
using Ambev.DeveloperEvaluation.WebApi.Features.Companies.DeleteCompany;
using Ambev.DeveloperEvaluation.WebApi.Features.Companies.GetCompany;
using Ambev.DeveloperEvaluation.WebApi.Features.Companies.ListCompanies;
using Ambev.DeveloperEvaluation.WebApi.Features.Companies.UpdateCompany;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Companies;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CompaniesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateCompanyResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateCompanyRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateCompanyCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateCompanyResponse>
        {
            Success = true,
            Message = "Company created successfully",
            Data = _mapper.Map<CreateCompanyResponse>(response)
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetCompanyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompany([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetCompanyRequest { Id = id };
        var validator = new GetCompanyRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetCompanyCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetCompanyResponse>
        {
            Success = true,
            Message = "Company retrieved successfully",
            Data = _mapper.Map<GetCompanyResponse>(response)
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<ListCompaniesResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListCompanies(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListCompaniesCommand(), cancellationToken);

        return Ok(new ApiResponseWithData<IEnumerable<ListCompaniesResponse>>
        {
            Success = true,
            Message = "Companies retrieved successfully",
            Data = _mapper.Map<IEnumerable<ListCompaniesResponse>>(response)
        });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateCompanyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompany([FromRoute] Guid id, [FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new UpdateCompanyRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateCompanyCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateCompanyResponse>
        {
            Success = true,
            Message = "Company updated successfully",
            Data = _mapper.Map<UpdateCompanyResponse>(response)
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCompany([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteCompanyRequest { Id = id };
        var validator = new DeleteCompanyRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteCompanyCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Company deleted successfully"
        });
    }
}
