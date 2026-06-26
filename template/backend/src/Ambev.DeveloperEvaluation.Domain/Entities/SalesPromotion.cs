using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SalesPromotion : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int Priority { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid? ProductId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public List<SalesPromotionItem> Items { get; private set; } = [];

    public SalesPromotion()
    {
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static SalesPromotion Create(
        string name,
        string? description,
        int priority,
        DateTime startDate,
        DateTime endDate,
        Guid? productId,
        bool isActive,
        IEnumerable<SalesPromotionItem> items)
    {
        var promotion = new SalesPromotion
        {
            CreatedAt = DateTime.UtcNow
        };

        promotion.SetDetails(name, description, priority, startDate, endDate, productId, isActive);
        promotion.ReplaceItems(items);
        return promotion;
    }

    public void Update(
        string name,
        string? description,
        int priority,
        DateTime startDate,
        DateTime endDate,
        Guid? productId,
        bool isActive,
        IEnumerable<SalesPromotionItem> items)
    {
        SetDetails(name, description, priority, startDate, endDate, productId, isActive);
        ReplaceItems(items);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsApplicable(Guid productId, decimal quantity, DateTime referenceDate)
    {
        return IsActive
            && StartDate <= referenceDate
            && EndDate >= referenceDate
            && (ProductId == null || ProductId == productId)
            && Items.Any(item => item.MatchesQuantity(quantity));
    }

    public ValidationResultDetail Validate()
    {
        var validator = new SalesPromotionValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }

    private void SetDetails(
        string name,
        string? description,
        int priority,
        DateTime startDate,
        DateTime endDate,
        Guid? productId,
        bool isActive)
    {
        ValidateBusinessRules(name, priority, startDate, endDate);

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Priority = priority;
        StartDate = startDate;
        EndDate = endDate;
        ProductId = productId == Guid.Empty ? null : productId;
        IsActive = isActive;
    }

    private void ReplaceItems(IEnumerable<SalesPromotionItem> items)
    {
        var normalizedItems = items.ToList();

        if (normalizedItems.Count == 0)
            throw new ArgumentException("Promotion must have at least one item.", nameof(items));

        EnsureNoOverlaps(normalizedItems);

        Items.Clear();
        foreach (var item in normalizedItems)
        {
            item.PromotionId = Id;
            Items.Add(item);
        }
    }

    private static void ValidateBusinessRules(
        string name,
        int priority,
        DateTime startDate,
        DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Promotion name is required.", nameof(name));

        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal to zero.", nameof(priority));

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be less than start date.", nameof(endDate));
    }

    private static void EnsureNoOverlaps(IReadOnlyList<SalesPromotionItem> items)
    {
        var orderedItems = items.OrderBy(item => item.MinimumQuantity).ToList();
        for (var i = 1; i < orderedItems.Count; i++)
        {
            var previous = orderedItems[i - 1];
            var current = orderedItems[i];

            if (current.MinimumQuantity <= previous.MaximumQuantity)
                throw new ArgumentException("Promotion items cannot contain overlapping quantity ranges.", nameof(items));
        }
    }
}
