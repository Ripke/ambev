using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.UpdateSalesPromotion;

public class UpdateSalesPromotionHandler : IRequestHandler<UpdateSalesPromotionCommand, UpdateSalesPromotionResult>
{
    private readonly ISalesPromotionRepository _salesPromotionRepository;
    private readonly IMapper _mapper;

    public UpdateSalesPromotionHandler(ISalesPromotionRepository salesPromotionRepository, IMapper mapper)
    {
        _salesPromotionRepository = salesPromotionRepository;
        _mapper = mapper;
    }

    public async Task<UpdateSalesPromotionResult> Handle(UpdateSalesPromotionCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSalesPromotionCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var promotion = await _salesPromotionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (promotion == null)
            throw new KeyNotFoundException($"Sales promotion with ID {request.Id} not found");

        promotion.Update(
            request.Name,
            request.Description,
            request.Priority,
            request.StartDate,
            request.EndDate,
            request.ProductId,
            request.IsActive,
            request.Items.Select(MapItem));

        await _salesPromotionRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<UpdateSalesPromotionResult>(promotion);
    }

    private static SalesPromotionItem MapItem(SalesPromotionItemInput item)
    {
        return SalesPromotionItem.Create(item.MinimumQuantity, item.MaximumQuantity, item.DiscountType, item.DiscountValue);
    }
}
