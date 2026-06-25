using Ambev.DeveloperEvaluation.Application.Companies.GetCompany;
using Ambev.DeveloperEvaluation.Application.Companies.ListCompanies;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Companies;

public class CompanyQueryHandlersTests
{
    [Fact]
    public async Task GetCompany_ShouldReturnFormattedCnpj()
    {
        var repository = Substitute.For<ICompanyRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new GetCompanyHandler(repository, mapper);
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);
        var result = new GetCompanyResult();

        repository.GetByIdAsync(company.Id, Arg.Any<CancellationToken>()).Returns(company);
        mapper.Map<GetCompanyResult>(company).Returns(result);

        var response = await handler.Handle(new GetCompanyCommand(company.Id), CancellationToken.None);

        response.Cnpj.Should().Be("11.222.333/0001-81");
    }

    [Fact]
    public async Task ListCompanies_ShouldReturnAllStatusesWithFormattedCnpj()
    {
        var repository = Substitute.For<ICompanyRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new ListCompaniesHandler(repository, mapper);
        var companies = new List<Company>
        {
            Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active),
            Company.Create("Bebidas", "19131243000197", CreateAddress(), CompanyStatus.Inactive)
        };
        var results = new List<ListCompaniesResult>
        {
            new() { Status = CompanyStatus.Active },
            new() { Status = CompanyStatus.Inactive }
        };

        repository.ListAsync(Arg.Any<CancellationToken>()).Returns(companies);
        mapper.Map<IReadOnlyList<ListCompaniesResult>>(companies).Returns(results);

        var response = await handler.Handle(new ListCompaniesCommand(), CancellationToken.None);

        response.Should().HaveCount(2);
        response[0].Cnpj.Should().Be("11.222.333/0001-81");
        response[1].Status.Should().Be(CompanyStatus.Inactive);
    }

    private static CompanyAddress CreateAddress()
    {
        return new("Rua A", "100", null, "Centro", "Sao Paulo", "SP", "01310100", "Brasil");
    }
}
