using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByCustomer;

public class GetCurrentSaleByCustomerHandler : IRequestHandler<GetCurrentSaleByCustomerCommand, GetCurrentSaleByCustomerResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetCurrentSaleByCustomerHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<GetCurrentSaleByCustomerResult> Handle(GetCurrentSaleByCustomerCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetCurrentSaleByCustomerValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetCurrentByCustomerIdAsync(request.CustomerId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Current sale for customer ID {request.CustomerId} not found");

        return _mapper.Map<GetCurrentSaleByCustomerResult>(sale);
    }
}
