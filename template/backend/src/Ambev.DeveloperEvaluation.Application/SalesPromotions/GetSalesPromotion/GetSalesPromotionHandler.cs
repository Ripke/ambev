using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.GetSalesPromotion;

public class GetSalesPromotionHandler : IRequestHandler<GetSalesPromotionCommand, GetSalesPromotionResult>
{
    private readonly ISalesPromotionRepository _salesPromotionRepository;
    private readonly IMapper _mapper;

    public GetSalesPromotionHandler(ISalesPromotionRepository salesPromotionRepository, IMapper mapper)
    {
        _salesPromotionRepository = salesPromotionRepository;
        _mapper = mapper;
    }

    public async Task<GetSalesPromotionResult> Handle(GetSalesPromotionCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSalesPromotionValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var promotion = await _salesPromotionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (promotion == null)
            throw new KeyNotFoundException($"Sales promotion with ID {request.Id} not found");

        return _mapper.Map<GetSalesPromotionResult>(promotion);
    }
}
