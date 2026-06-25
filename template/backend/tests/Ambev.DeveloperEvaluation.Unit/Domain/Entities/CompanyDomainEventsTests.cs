using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class CompanyDomainEventsTests
{
    [Fact]
    public void Create_ShouldRegisterCreatedEvent()
    {
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);

        company.Events.Should().ContainSingle(x => x is CompanyCreatedEvent);
    }

    [Fact]
    public void Update_ShouldRegisterUpdatedEvent()
    {
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);

        company.Update("Ambev SA", "11222333000181", CreateAddress(), CompanyStatus.Active);

        company.Events.Should().ContainSingle(x => x is CompanyUpdatedEvent);
    }

    [Fact]
    public void Block_ShouldRegisterStatusChangedEvent()
    {
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);

        company.DequeueEvents();
        company.Block();

        company.Events.Should().ContainSingle(x => x is CompanyStatusChangedEvent);
    }

    [Fact]
    public void Create_WithUnknownStatus_ShouldThrow()
    {
        var act = () => Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Unknown);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidCnpj_ShouldThrow()
    {
        var act = () => Company.Create("Ambev", "123", CreateAddress(), CompanyStatus.Active);

        act.Should().Throw<ArgumentException>();
    }

    private static CompanyAddress CreateAddress()
    {
        return new CompanyAddress("Rua A", "100", null, "Centro", "Sao Paulo", "SP", "01310100", "Brasil");
    }
}
