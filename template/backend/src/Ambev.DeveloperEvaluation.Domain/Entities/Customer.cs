using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public string EncryptedCpf { get; private set; } = string.Empty;
    public CustomerStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Customer()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static Customer Create(
        string fullName,
        DateTime birthDate,
        string plainCpf,
        string encryptedCpf,
        CustomerStatus status)
    {
        var customer = new Customer
        {
            CreatedAt = DateTime.UtcNow
        };

        customer.SetDetails(fullName, birthDate, plainCpf, encryptedCpf, status);
        customer.AddEvent(new CustomerCreatedEvent(customer.Id));
        return customer;
    }

    public void Update(
        string fullName,
        DateTime birthDate,
        string plainCpf,
        string encryptedCpf,
        CustomerStatus status)
    {
        SetDetails(fullName, birthDate, plainCpf, encryptedCpf, status);
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new CustomerUpdatedEvent(Id, UpdatedAt.Value));
    }

    public void Activate()
    {
        ChangeStatus(CustomerStatus.Active);
    }

    public void Deactivate()
    {
        ChangeStatus(CustomerStatus.Inactive);
    }

    public void Block()
    {
        ChangeStatus(CustomerStatus.Blocked);
    }

    public ValidationResultDetail Validate()
    {
        var validator = new CustomerValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }

    private void SetDetails(
        string fullName,
        DateTime birthDate,
        string plainCpf,
        string encryptedCpf,
        CustomerStatus status)
    {
        ValidateBusinessRules(fullName, birthDate, plainCpf, encryptedCpf, status);

        FullName = fullName.Trim();
        BirthDate = birthDate.Date;
        EncryptedCpf = encryptedCpf;
        Status = status;
    }

    private void ChangeStatus(CustomerStatus status)
    {
        if (Status == status)
            return;

        Status = status;
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new CustomerStatusChangedEvent(Id, Status, UpdatedAt.Value));
    }

    private static void ValidateBusinessRules(
        string fullName,
        DateTime birthDate,
        string plainCpf,
        string encryptedCpf,
        CustomerStatus status)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        if (birthDate.Date > DateTime.UtcNow.Date)
            throw new ArgumentException("Birth date cannot be in the future.", nameof(birthDate));

        var cpfValidator = new CpfValidator();
        var cpfValidationResult = cpfValidator.Validate(plainCpf);
        if (!cpfValidationResult.IsValid)
            throw new ArgumentException(cpfValidationResult.Errors[0].ErrorMessage, nameof(plainCpf));

        if (string.IsNullOrWhiteSpace(encryptedCpf))
            throw new ArgumentException("Encrypted CPF is required.", nameof(encryptedCpf));

        if (status == CustomerStatus.Unknown)
            throw new ArgumentException("Customer status cannot be Unknown.", nameof(status));
    }
}
