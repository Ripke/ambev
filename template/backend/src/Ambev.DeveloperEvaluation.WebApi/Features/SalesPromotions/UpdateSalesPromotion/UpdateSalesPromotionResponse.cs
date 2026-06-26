namespace Ambev.DeveloperEvaluation.WebApi.Features.SalesPromotions.UpdateSalesPromotion;

public class UpdateSalesPromotionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? ProductId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<SalesPromotionItemResponse> Items { get; set; } = [];
}
