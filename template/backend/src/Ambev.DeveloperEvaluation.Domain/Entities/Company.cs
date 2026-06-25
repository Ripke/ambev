using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Cnpj { get; private set; } = string.Empty;
    public CompanyAddress Address { get; private set; } = new();
    public CompanyStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Company()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static Company Create(
        string name,
        string cnpj,
        CompanyAddress address,
        CompanyStatus status)
    {
        var company = new Company
        {
            CreatedAt = DateTime.UtcNow
        };

        company.SetDetails(name, cnpj, address, status);
        company.AddEvent(new CompanyCreatedEvent(company.Id));
        return company;
    }

    public void Update(
        string name,
        string cnpj,
        CompanyAddress address,
        CompanyStatus status)
    {
        SetDetails(name, cnpj, address, status);
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new CompanyUpdatedEvent(Id, UpdatedAt.Value));
    }

    public void Activate()
    {
        ChangeStatus(CompanyStatus.Active);
    }

    public void Block()
    {
        ChangeStatus(CompanyStatus.Blocked);
    }

    public void Deactivate()
    {
        ChangeStatus(CompanyStatus.Inactive);
    }

    public ValidationResultDetail Validate()
    {
        var validator = new CompanyValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }

    private void SetDetails(
        string name,
        string cnpj,
        CompanyAddress address,
        CompanyStatus status)
    {
        ValidateBusinessRules(name, cnpj, address, status);

        Name = name.Trim();
        Cnpj = CnpjHelper.Normalize(cnpj);
        Address = new CompanyAddress(
            address.Street,
            address.Number,
            address.Complement,
            address.District,
            address.City,
            address.State,
            address.ZipCode,
            address.Country);
        Status = status;
    }

    private void ChangeStatus(CompanyStatus status)
    {
        if (Status == status)
            return;

        Status = status;
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new CompanyStatusChangedEvent(Id, Status, UpdatedAt.Value));
    }

    private static void ValidateBusinessRules(
        string name,
        string cnpj,
        CompanyAddress address,
        CompanyStatus status)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name is required.", nameof(name));

        if (name.Trim().Length > 200)
            throw new ArgumentException("Company name cannot exceed 200 characters.", nameof(name));

        var cnpjValidator = new CnpjValidator();
        var cnpjValidationResult = cnpjValidator.Validate(cnpj);
        if (!cnpjValidationResult.IsValid)
            throw new ArgumentException(cnpjValidationResult.Errors[0].ErrorMessage, nameof(cnpj));

        if (address == null)
            throw new ArgumentNullException(nameof(address), "Company address is required.");

        var addressValidator = new CompanyAddressValidator();
        var addressValidationResult = addressValidator.Validate(address);
        if (!addressValidationResult.IsValid)
            throw new ArgumentException(addressValidationResult.Errors[0].ErrorMessage, nameof(address));

        if (status == CompanyStatus.Unknown)
            throw new ArgumentException("Company status cannot be Unknown.", nameof(status));
    }
}
