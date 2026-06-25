using Ambev.DeveloperEvaluation.Application.Customers.GetCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.ListCustomers;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Customers;

public class CustomerQueryHandlersTests
{
    [Fact]
    public async Task GetCustomer_ShouldReturnMaskedCpfOnly()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var cpfProtectionService = Substitute.For<ICpfProtectionService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new GetCustomerHandler(repository, cpfProtectionService, mapper);
        var customer = Customer.Create("Maria Silva", new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), "529.982.247-25", "encrypted", CustomerStatus.Active);
        var result = new GetCustomerResult();

        repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        cpfProtectionService.Decrypt("encrypted").Returns("52998224725");
        cpfProtectionService.Mask("52998224725").Returns("529.xxx.xxx-25");
        mapper.Map<GetCustomerResult>(customer).Returns(result);

        var response = await handler.Handle(new GetCustomerCommand(customer.Id), CancellationToken.None);

        response.MaskedCpf.Should().Be("529.xxx.xxx-25");
    }

    [Fact]
    public async Task ListCustomers_ShouldReturnMaskedCpfOnly()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var cpfProtectionService = Substitute.For<ICpfProtectionService>();
        var mapper = Substitute.For<IMapper>();
        var handler = new ListCustomersHandler(repository, cpfProtectionService, mapper);
        var customers = new List<Customer>
        {
            Customer.Create("Maria Silva", new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), "529.982.247-25", "encrypted", CustomerStatus.Active)
        };
        var results = new List<ListCustomersResult>
        {
            new()
        };

        repository.ListAsync(Arg.Any<CancellationToken>()).Returns(customers);
        mapper.Map<IReadOnlyList<ListCustomersResult>>(customers).Returns(results);
        cpfProtectionService.Decrypt("encrypted").Returns("52998224725");
        cpfProtectionService.Mask("52998224725").Returns("529.xxx.xxx-25");

        var response = await handler.Handle(new ListCustomersCommand(), CancellationToken.None);

        response[0].MaskedCpf.Should().Be("529.xxx.xxx-25");
    }
}
