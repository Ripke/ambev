using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetCurrentSaleByUser;

public class GetCurrentSaleByUserHandler : IRequestHandler<GetCurrentSaleByUserCommand, GetCurrentSaleByUserResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetCurrentSaleByUserHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<GetCurrentSaleByUserResult> Handle(GetCurrentSaleByUserCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetCurrentSaleByUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetCurrentByUserIdAsync(request.UserId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Current sale for user ID {request.UserId} not found");

        return _mapper.Map<GetCurrentSaleByUserResult>(sale);
    }
}
