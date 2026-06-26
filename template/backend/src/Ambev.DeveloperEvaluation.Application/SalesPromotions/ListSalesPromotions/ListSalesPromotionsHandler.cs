using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.ListSalesPromotions;

public class ListSalesPromotionsHandler : IRequestHandler<ListSalesPromotionsCommand, IReadOnlyList<ListSalesPromotionsResult>>
{
    private readonly ISalesPromotionRepository _salesPromotionRepository;
    private readonly IMapper _mapper;

    public ListSalesPromotionsHandler(ISalesPromotionRepository salesPromotionRepository, IMapper mapper)
    {
        _salesPromotionRepository = salesPromotionRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListSalesPromotionsResult>> Handle(ListSalesPromotionsCommand request, CancellationToken cancellationToken)
    {
        var promotions = await _salesPromotionRepository.ListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ListSalesPromotionsResult>>(promotions);
    }
}
