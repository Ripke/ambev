using Ambev.DeveloperEvaluation.Application.Customers.CreateCustomer;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Customers;

public class CreateCustomerHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICpfProtectionService _cpfProtectionService;
    private readonly IMapper _mapper;
    private readonly CreateCustomerHandler _handler;

    public CreateCustomerHandlerTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _cpfProtectionService = Substitute.For<ICpfProtectionService>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateCustomerHandler(_customerRepository, _cpfProtectionService, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsMaskedCustomer()
    {
        var command = CreateCommand();
        var customer = Customer.Create(command.FullName, command.BirthDate, command.Cpf, "encrypted", command.Status);
        var result = new CreateCustomerResult();

        _cpfProtectionService.Encrypt(command.Cpf).Returns("encrypted");
        _cpfProtectionService.Mask(command.Cpf).Returns("529.xxx.xxx-25");
        _customerRepository.CreateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.Arg<Customer>());
        _mapper.Map<CreateCustomerResult>(Arg.Any<Customer>()).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        response.MaskedCpf.Should().Be("529.xxx.xxx-25");
        await _customerRepository.Received(1).CreateAsync(
            Arg.Is<Customer>(x => x.FullName == command.FullName && x.EncryptedCpf == "encrypted"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidCpf_ThrowsValidationException()
    {
        var command = CreateCommand();
        command.Cpf = "123";

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_FutureBirthDate_ThrowsValidationException()
    {
        var command = CreateCommand();
        command.BirthDate = DateTime.UtcNow.Date.AddDays(1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    private static CreateCustomerCommand CreateCommand()
    {
        return new CreateCustomerCommand
        {
            FullName = "Maria Silva",
            BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Cpf = "529.982.247-25",
            Status = CustomerStatus.Active
        };
    }
}
