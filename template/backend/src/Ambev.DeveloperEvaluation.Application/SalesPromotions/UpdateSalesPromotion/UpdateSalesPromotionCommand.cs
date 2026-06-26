using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesPromotions.UpdateSalesPromotion;

public class UpdateSalesPromotionCommand : IRequest<UpdateSalesPromotionResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? ProductId { get; set; }
    public bool IsActive { get; set; }
    public List<SalesPromotionItemInput> Items { get; set; } = [];
}
