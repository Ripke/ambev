using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.CreateSalesPromotion;

public class CreateSalesPromotionHandler : IRequestHandler<CreateSalesPromotionCommand, CreateSalesPromotionResult>
{
    private readonly ISalesPromotionRepository _salesPromotionRepository;
    private readonly IMapper _mapper;

    public CreateSalesPromotionHandler(ISalesPromotionRepository salesPromotionRepository, IMapper mapper)
    {
        _salesPromotionRepository = salesPromotionRepository;
        _mapper = mapper;
    }

    public async Task<CreateSalesPromotionResult> Handle(CreateSalesPromotionCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateSalesPromotionCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var promotion = SalesPromotion.Create(
            request.Name,
            request.Description,
            request.Priority,
            request.StartDate,
            request.EndDate,
            request.ProductId,
            request.IsActive,
            request.Items.Select(MapItem));

        var createdPromotion = await _salesPromotionRepository.CreateAsync(promotion, cancellationToken);
        return _mapper.Map<CreateSalesPromotionResult>(createdPromotion);
    }

    private static SalesPromotionItem MapItem(SalesPromotionItemInput item)
    {
        return SalesPromotionItem.Create(item.MinimumQuantity, item.MaximumQuantity, item.DiscountType, item.DiscountValue);
    }
}
