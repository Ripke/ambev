using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.GetCustomer;

public class GetCustomerHandler : IRequestHandler<GetCustomerCommand, GetCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICpfProtectionService _cpfProtectionService;
    private readonly IMapper _mapper;

    public GetCustomerHandler(
        ICustomerRepository customerRepository,
        ICpfProtectionService cpfProtectionService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _cpfProtectionService = cpfProtectionService;
        _mapper = mapper;
    }

    public async Task<GetCustomerResult> Handle(GetCustomerCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetCustomerValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
            throw new KeyNotFoundException($"Customer with ID {request.Id} not found");

        var result = _mapper.Map<GetCustomerResult>(customer);
        result.MaskedCpf = _cpfProtectionService.Mask(_cpfProtectionService.Decrypt(customer.EncryptedCpf));
        return result;
    }
}
