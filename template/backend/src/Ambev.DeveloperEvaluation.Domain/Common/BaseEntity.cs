using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Common;

public class BaseEntity : IComparable<BaseEntity>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyCollection<IDomainEvent> Events => _events;

    public Task<IEnumerable<ValidationErrorDetail>> ValidateAsync()
    {
        return Validator.ValidateAsync(this);
    }

    protected void AddEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    protected internal void ClearEvents() => _events.Clear();

    public IReadOnlyCollection<IDomainEvent> DequeueEvents()
    {
        var domainEvents = _events.ToList().AsReadOnly();
        _events.Clear();
        return domainEvents;
    }

    public int CompareTo(BaseEntity? other)
    {
        if (other == null)
        {
            return 1;
        }

        return other!.Id.CompareTo(Id);
    }
}
