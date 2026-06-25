using Ambev.DeveloperEvaluation.Application.Companies;
using Ambev.DeveloperEvaluation.Application.Companies.UpdateCompany;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Companies;

public class UpdateCompanyHandlerTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly UpdateCompanyHandler _handler;

    public UpdateCompanyHandlerTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateCompanyHandler(_companyRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesCompanyAndReturnsFormattedCnpj()
    {
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);
        var command = new UpdateCompanyCommand
        {
            Id = company.Id,
            Name = "Ambev SA",
            Cnpj = "11.222.333/0001-81",
            Address = CreateAddressCommand(),
            Status = CompanyStatus.Blocked
        };
        var result = new UpdateCompanyResult();

        _companyRepository.GetByIdAsync(company.Id, Arg.Any<CancellationToken>()).Returns(company);
        _companyRepository.GetByCnpjAsync("11222333000181", Arg.Any<CancellationToken>()).Returns(company);
        _mapper.Map<UpdateCompanyResult>(company).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        await _companyRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        response.Cnpj.Should().Be("11.222.333/0001-81");
        company.Status.Should().Be(CompanyStatus.Blocked);
        company.Name.Should().Be("Ambev SA");
    }

    [Fact]
    public async Task Handle_DuplicateCnpj_ThrowsValidationException()
    {
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);
        var existingCompany = Company.Create("Outra", "19131243000197", CreateAddress(), CompanyStatus.Active);
        var command = new UpdateCompanyCommand
        {
            Id = company.Id,
            Name = "Ambev SA",
            Cnpj = "19.131.243/0001-97",
            Address = CreateAddressCommand(),
            Status = CompanyStatus.Active
        };

        _companyRepository.GetByIdAsync(company.Id, Arg.Any<CancellationToken>()).Returns(company);
        _companyRepository.GetByCnpjAsync("19131243000197", Arg.Any<CancellationToken>()).Returns(existingCompany);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    private static CompanyAddress CreateAddress()
    {
        return new("Rua A", "100", null, "Centro", "Sao Paulo", "SP", "01310100", "Brasil");
    }

    private static CompanyAddressCommand CreateAddressCommand()
    {
        return new CompanyAddressCommand
        {
            Street = "Rua A",
            Number = "100",
            District = "Centro",
            City = "Sao Paulo",
            State = "SP",
            ZipCode = "01310-100",
            Country = "Brasil"
        };
    }
}
