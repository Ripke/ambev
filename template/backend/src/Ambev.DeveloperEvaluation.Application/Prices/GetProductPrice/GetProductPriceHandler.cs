using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Prices.GetProductPrice;

public class GetProductPriceHandler : IRequestHandler<GetProductPriceCommand, GetProductPriceResult>
{
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IMapper _mapper;

    public GetProductPriceHandler(IProductPriceRepository productPriceRepository, IMapper mapper)
    {
        _productPriceRepository = productPriceRepository;
        _mapper = mapper;
    }

    public async Task<GetProductPriceResult> Handle(GetProductPriceCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetProductPriceValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var price = await _productPriceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (price == null)
            throw new KeyNotFoundException($"Price with ID {request.Id} not found");

        return _mapper.Map<GetProductPriceResult>(price);
    }
}
