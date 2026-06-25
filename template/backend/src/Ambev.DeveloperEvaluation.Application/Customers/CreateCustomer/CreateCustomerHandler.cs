using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.CreateCustomer;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICpfProtectionService _cpfProtectionService;
    private readonly IMapper _mapper;

    public CreateCustomerHandler(
        ICustomerRepository customerRepository,
        ICpfProtectionService cpfProtectionService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _cpfProtectionService = cpfProtectionService;
        _mapper = mapper;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCustomerCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var customer = Domain.Entities.Customer.Create(
            command.FullName,
            command.BirthDate,
            command.Cpf,
            _cpfProtectionService.Encrypt(command.Cpf),
            command.Status);

        var createdCustomer = await _customerRepository.CreateAsync(customer, cancellationToken);
        var result = _mapper.Map<CreateCustomerResult>(createdCustomer);
        result.MaskedCpf = _cpfProtectionService.Mask(command.Cpf);
        return result;
    }
}
