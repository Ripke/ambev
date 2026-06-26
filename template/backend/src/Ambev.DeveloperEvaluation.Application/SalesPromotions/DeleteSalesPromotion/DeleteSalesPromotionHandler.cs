using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.DeleteSalesPromotion;

public class DeleteSalesPromotionHandler : IRequestHandler<DeleteSalesPromotionCommand, DeleteSalesPromotionResponse>
{
    private readonly ISalesPromotionRepository _salesPromotionRepository;

    public DeleteSalesPromotionHandler(ISalesPromotionRepository salesPromotionRepository)
    {
        _salesPromotionRepository = salesPromotionRepository;
    }

    public async Task<DeleteSalesPromotionResponse> Handle(DeleteSalesPromotionCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSalesPromotionValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var promotion = await _salesPromotionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (promotion == null)
            throw new KeyNotFoundException($"Sales promotion with ID {request.Id} not found");

        promotion.Deactivate();
        await _salesPromotionRepository.SaveChangesAsync(cancellationToken);

        return new DeleteSalesPromotionResponse { Success = true };
    }
}
