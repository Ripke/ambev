namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public class CompanyAddress
{
    public string Street { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public string Complement { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;

    public CompanyAddress()
    {
    }

    public CompanyAddress(
        string street,
        string number,
        string? complement,
        string district,
        string city,
        string state,
        string zipCode,
        string country)
    {
        Update(street, number, complement, district, city, state, zipCode, country);
    }

    public void Update(
        string street,
        string number,
        string? complement,
        string district,
        string city,
        string state,
        string zipCode,
        string country)
    {
        Street = street.Trim();
        Number = number.Trim();
        Complement = complement?.Trim() ?? string.Empty;
        District = district.Trim();
        City = city.Trim();
        State = state.Trim();
        ZipCode = zipCode.Trim();
        Country = country.Trim();
    }
}
