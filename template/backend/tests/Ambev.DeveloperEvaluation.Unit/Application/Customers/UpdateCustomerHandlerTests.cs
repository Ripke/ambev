using Ambev.DeveloperEvaluation.Application.Customers.UpdateCustomer;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Customers;

public class UpdateCustomerHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICpfProtectionService _cpfProtectionService;
    private readonly IMapper _mapper;
    private readonly UpdateCustomerHandler _handler;

    public UpdateCustomerHandlerTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _cpfProtectionService = Substitute.For<ICpfProtectionService>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateCustomerHandler(_customerRepository, _cpfProtectionService, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesCustomerAndReturnsMaskedCpf()
    {
        var customer = Customer.Create("Maria Silva", new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), "529.982.247-25", "encrypted", CustomerStatus.Active);
        var command = new UpdateCustomerCommand
        {
            Id = customer.Id,
            FullName = "Maria Souza",
            BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Cpf = "529.982.247-25",
            Status = CustomerStatus.Blocked
        };
        var result = new UpdateCustomerResult();

        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _cpfProtectionService.Encrypt(command.Cpf).Returns("encrypted-2");
        _cpfProtectionService.Mask(command.Cpf).Returns("529.xxx.xxx-25");
        _mapper.Map<UpdateCustomerResult>(customer).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        await _customerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        response.MaskedCpf.Should().Be("529.xxx.xxx-25");
        customer.Status.Should().Be(CustomerStatus.Blocked);
    }
}
