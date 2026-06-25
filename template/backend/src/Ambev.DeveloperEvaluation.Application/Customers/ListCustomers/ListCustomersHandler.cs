using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.ListCustomers;

public class ListCustomersHandler : IRequestHandler<ListCustomersCommand, IReadOnlyList<ListCustomersResult>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICpfProtectionService _cpfProtectionService;
    private readonly IMapper _mapper;

    public ListCustomersHandler(
        ICustomerRepository customerRepository,
        ICpfProtectionService cpfProtectionService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _cpfProtectionService = cpfProtectionService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListCustomersResult>> Handle(ListCustomersCommand request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.ListAsync(cancellationToken);
        var results = _mapper.Map<IReadOnlyList<ListCustomersResult>>(customers);

        for (var index = 0; index < customers.Count; index++)
        {
            results[index].MaskedCpf = _cpfProtectionService.Mask(_cpfProtectionService.Decrypt(customers[index].EncryptedCpf));
        }

        return results;
    }
}
