using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.UpdateCustomer;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICpfProtectionService _cpfProtectionService;
    private readonly IMapper _mapper;

    public UpdateCustomerHandler(
        ICustomerRepository customerRepository,
        ICpfProtectionService cpfProtectionService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _cpfProtectionService = cpfProtectionService;
        _mapper = mapper;
    }

    public async Task<UpdateCustomerResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateCustomerCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
            throw new KeyNotFoundException($"Customer with ID {request.Id} not found");

        customer.Update(
            request.FullName,
            request.BirthDate,
            request.Cpf,
            _cpfProtectionService.Encrypt(request.Cpf),
            request.Status);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<UpdateCustomerResult>(customer);
        result.MaskedCpf = _cpfProtectionService.Mask(request.Cpf);
        return result;
    }
}
