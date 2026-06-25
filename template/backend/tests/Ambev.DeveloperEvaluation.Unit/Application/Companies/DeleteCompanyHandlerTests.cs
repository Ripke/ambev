using Ambev.DeveloperEvaluation.Application.Companies.DeleteCompany;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Companies;

public class DeleteCompanyHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_DeactivatesCompany()
    {
        var repository = Substitute.For<ICompanyRepository>();
        var handler = new DeleteCompanyHandler(repository);
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);

        repository.GetByIdAsync(company.Id, Arg.Any<CancellationToken>()).Returns(company);

        var response = await handler.Handle(new DeleteCompanyCommand(company.Id), CancellationToken.None);

        response.Success.Should().BeTrue();
        company.Status.Should().Be(CompanyStatus.Inactive);
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static CompanyAddress CreateAddress()
    {
        return new("Rua A", "100", null, "Centro", "Sao Paulo", "SP", "01310100", "Brasil");
    }
}
