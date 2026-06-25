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
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _companyRepository = Substitute.For<ICompanyRepository>();
        _customerRepository = Substitute.For<ICustomerRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _companyRepository, _customerRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesSaleWithDenormalizedNames()
    {
        var command = new CreateSaleCommand
        {
            CompanyId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid()
        };
        var result = new CreateSaleResult();
        var company = Company.Create("Ambev", "11222333000181", CreateAddress(), CompanyStatus.Active);
        var customer = Customer.Create("Maria Silva", new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), "529.982.247-25", "encrypted", CustomerStatus.Active);

        _saleRepository.ExistsOpenSaleByCustomerIdAsync(command.CustomerId, Arg.Any<CancellationToken>()).Returns(false);
        _companyRepository.GetByIdAsync(command.CompanyId, Arg.Any<CancellationToken>()).Returns(company);
        _customerRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>()).Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(call => call.Arg<Sale>());
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(sale => sale.CompanyName == "Ambev" && sale.CustomerName == "Maria Silva"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerHasCurrentSale_ThrowsValidationException()
    {
        var command = new CreateSaleCommand
        {
            CompanyId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid()
        };

        _saleRepository.ExistsOpenSaleByCustomerIdAsync(command.CustomerId, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    private static CompanyAddress CreateAddress()
    {
        return new("Rua A", "100", null, "Centro", "Sao Paulo", "SP", "01310100", "Brasil");
    }
}
