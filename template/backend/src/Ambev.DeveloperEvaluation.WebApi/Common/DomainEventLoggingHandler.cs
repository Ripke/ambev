using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class DomainEventLoggingHandler : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<DomainEventLoggingHandler> _logger;

    public DomainEventLoggingHandler(ILogger<DomainEventLoggingHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain event published: {DomainEventType} at {OccurredAt} with payload {@DomainEvent}",
            notification.DomainEvent.GetType().Name,
            notification.DomainEvent.OccurredAt,
            notification.DomainEvent);

        return Task.CompletedTask;
    }
}
