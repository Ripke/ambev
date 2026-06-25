using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed record ProductPriceUpdatedEvent(
    Guid ProductId,
    PriceType PriceType,
    decimal Price,
    DateTime UpdatedAt) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
