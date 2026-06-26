namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.CreateSalesPromotion;

public class CreateSalesPromotionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? ProductId { get; set; }
    public bool IsActive { get; set; } = true;
    public List<SalesPromotionItemRequest> Items { get; set; } = [];
}
