using Ambev.DeveloperEvaluation.Domain.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public sealed record DomainEventNotification(IDomainEvent DomainEvent) : INotification;
