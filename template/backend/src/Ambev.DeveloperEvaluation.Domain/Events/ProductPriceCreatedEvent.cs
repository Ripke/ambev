using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public sealed record ProductPriceCreatedEvent(
    Guid ProductId,
    PriceType PriceType,
    decimal Price,
    DateTime EffectiveStartAt,
    DateTime EffectiveEndAt) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
