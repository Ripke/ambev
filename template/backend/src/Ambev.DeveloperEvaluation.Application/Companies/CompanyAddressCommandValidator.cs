using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Companies;

public class CompanyAddressCommandValidator : AbstractValidator<CompanyAddressCommand>
{
    public CompanyAddressCommandValidator()
    {
        RuleFor(address => address.Street).NotEmpty().MaximumLength(200);
        RuleFor(address => address.Number).NotEmpty().MaximumLength(20);
        RuleFor(address => address.Complement).MaximumLength(100);
        RuleFor(address => address.District).NotEmpty().MaximumLength(100);
        RuleFor(address => address.City).NotEmpty().MaximumLength(100);
        RuleFor(address => address.State).NotEmpty().MaximumLength(2);
        RuleFor(address => address.ZipCode)
            .NotEmpty()
            .Must(zipCode => new string(zipCode.Where(char.IsDigit).ToArray()).Length == 8)
            .WithMessage("Zip code must contain 8 digits.");
        RuleFor(address => address.Country).NotEmpty().MaximumLength(100);
    }
}
