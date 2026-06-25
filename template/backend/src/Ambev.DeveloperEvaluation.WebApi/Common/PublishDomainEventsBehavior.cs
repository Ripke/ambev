using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class PublishDomainEventsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly DefaultContext _context;
    private readonly IMediator _mediator;

    public PublishDomainEventsBehavior(DefaultContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        var entitiesWithEvents = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.State != EntityState.Detached && entry.Entity.Events.Any())
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(entity => entity.DequeueEvents())
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(new DomainEventNotification(domainEvent), cancellationToken);
        }

        return response;
    }
}
