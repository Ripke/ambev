using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _companyRepository = Substitute.For<ICompanyRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _companyRepository, _userRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesSaleWithDenormalizedNames()
    {
        var command = new CreateSaleCommand
        {
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        var result = new CreateSaleResult();
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);
        var user = new User
        {
            Id = command.UserId,
            Username = "Maria Silva",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        _saleRepository.ExistsOpenSaleByUserIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(false);
        _companyRepository.GetByIdAsync(command.CompanyId, Arg.Any<CancellationToken>()).Returns(company);
        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(user);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(call => call.Arg<Sale>());
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(sale => sale.CompanyName == "Ambev" && sale.UserName == "Maria Silva"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserHasCurrentSale_ThrowsValidationException()
    {
        var command = new CreateSaleCommand
        {
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _saleRepository.ExistsOpenSaleByUserIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    private static CompanyAddress CreateAddress()
    {
        return new("Rua A", "100", null, "Centro", "Sao Paulo", "SP", "01310100", "Brasil");
    }
}
