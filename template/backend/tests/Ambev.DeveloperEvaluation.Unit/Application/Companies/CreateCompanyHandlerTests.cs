using Ambev.DeveloperEvaluation.Application.Companies;
using Ambev.DeveloperEvaluation.Application.Companies.CreateCompany;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Companies;

public class CreateCompanyHandlerTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly CreateCompanyHandler _handler;

    public CreateCompanyHandlerTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateCompanyHandler(_companyRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsFormattedCompany()
    {
        var command = CreateCommand();
        var result = new CreateCompanyResult();

        _companyRepository.GetByCnpjAsync("11222333000181", Arg.Any<CancellationToken>()).Returns((Company?)null);
        _companyRepository.CreateAsync(Arg.Any<Company>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.Arg<Company>());
        _mapper.Map<CreateCompanyResult>(Arg.Any<Company>()).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        response.Cnpj.Should().Be("11.222.333/0001-81");
        await _companyRepository.Received(1).CreateAsync(
            Arg.Is<Company>(x => x.Name == command.Name && x.Cnpj == "11222333000181"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateCnpj_ThrowsValidationException()
    {
        var command = CreateCommand();

        _companyRepository.GetByCnpjAsync("11222333000181", Arg.Any<CancellationToken>())
            .Returns(Company.Create("Existing", "11222333000181", CreateAddressEntity(), CompanyStatus.Active));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_InvalidCnpj_ThrowsValidationException()
    {
        var command = CreateCommand();
        command.Cnpj = "123";

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    private static CreateCompanyCommand CreateCommand()
    {
        return new CreateCompanyCommand
        {
            Name = "Ambev",
            Cnpj = "11.222.333/0001-81",
            Address = CreateAddressCommand(),
            Status = CompanyStatus.Active
        };
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

    private static Ambev.DeveloperEvaluation.Domain.ValueObjects.CompanyAddress CreateAddressEntity()
    {
        return new("Rua A", "100", null, "Centro", "Sao Paulo", "SP", "01310100", "Brasil");
    }
}
