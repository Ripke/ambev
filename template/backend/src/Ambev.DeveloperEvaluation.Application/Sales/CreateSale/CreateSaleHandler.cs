using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        ICompanyRepository companyRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _companyRepository = companyRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (await _saleRepository.ExistsOpenSaleByUserIdAsync(command.UserId, cancellationToken))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(command.UserId), "User already has a current sale.")
            });
        }

        var company = await _companyRepository.GetByIdAsync(command.CompanyId, cancellationToken);
        if (company == null)
            throw new KeyNotFoundException($"Company with ID {command.CompanyId} not found");

        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {command.UserId} not found");

        var sale = Domain.Entities.Sale.Create(
            command.CompanyId,
            company.Name,
            command.UserId,
            user.Username);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}
