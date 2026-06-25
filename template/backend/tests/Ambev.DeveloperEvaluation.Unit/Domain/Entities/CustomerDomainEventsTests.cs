using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class CustomerDomainEventsTests
{
    [Fact]
    public void Create_ShouldRegisterCreatedEvent()
    {
        var customer = Customer.Create(
            "Maria Silva",
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "529.982.247-25",
            "encrypted",
            CustomerStatus.Active);

        customer.Events.Should().ContainSingle(x => x is CustomerCreatedEvent);
    }

    [Fact]
    public void Update_ShouldRegisterUpdatedEvent()
    {
        var customer = Customer.Create(
            "Maria Silva",
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "529.982.247-25",
            "encrypted",
            CustomerStatus.Active);

        customer.Update(
            "Maria Souza",
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "529.982.247-25",
            "encrypted-2",
            CustomerStatus.Active);

        customer.Events.Should().ContainSingle(x => x is CustomerUpdatedEvent);
    }

    [Fact]
    public void Block_ShouldRegisterStatusChangedEvent()
    {
        var customer = Customer.Create(
            "Maria Silva",
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "529.982.247-25",
            "encrypted",
            CustomerStatus.Active);

        customer.DequeueEvents();
        customer.Block();

        customer.Events.Should().ContainSingle(x => x is CustomerStatusChangedEvent);
    }
}
