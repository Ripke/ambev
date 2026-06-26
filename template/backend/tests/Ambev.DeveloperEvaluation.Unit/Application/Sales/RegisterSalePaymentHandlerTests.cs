using Ambev.DeveloperEvaluation.Application.Sales.RegisterSalePayment;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class RegisterSalePaymentHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly RegisterSalePaymentHandler _handler;

    public RegisterSalePaymentHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new RegisterSalePaymentHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidVersion_ShouldPersistPaymentAndReturnUpdatedSale()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 25);
        sale.Subtotalize();
        var command = new RegisterSalePaymentCommand
        {
            SaleId = sale.Id,
            Version = sale.Version,
            TypePayment = PaymentType.Cash,
            Value = 10
        };
        var result = new RegisterSalePaymentResult();

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<RegisterSalePaymentResult>(sale).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        sale.PaymentAmountTotal.Should().Be(10);
        sale.Status.Should().Be(SaleStatus.PaymentCompleted);
        await _saleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOutdatedVersion_ShouldThrowValidationException()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 25);
        sale.Subtotalize();

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => _handler.Handle(new RegisterSalePaymentCommand
        {
            SaleId = sale.Id,
            Version = Guid.NewGuid(),
            TypePayment = PaymentType.Cash,
            Value = 10
        }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(error => error.ErrorMessage == "Sale version is outdated."));
    }

    [Fact]
    public async Task Handle_WithNonCashOverpayment_ShouldThrowValidationException()
    {
        var sale = Sale.Create(Guid.NewGuid(), "Ambev", Guid.NewGuid(), "Maria");
        sale.AddItem("789", Guid.NewGuid(), "Produto", 2, 50);
        sale.Subtotalize();
        sale.RegisterPayment(PaymentType.Cash, 95);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var act = () => _handler.Handle(new RegisterSalePaymentCommand
        {
            SaleId = sale.Id,
            Version = sale.Version,
            TypePayment = PaymentType.CreditCard,
            Value = 10
        }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(error => error.ErrorMessage == "Only cash payments can exceed the sale total."));
    }
}
