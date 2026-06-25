using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class CpfValidator : AbstractValidator<string>
{
    public CpfValidator()
    {
        RuleFor(cpf => cpf)
            .NotEmpty()
            .Must(BeValidCpf)
            .WithMessage("The provided CPF is not valid.");
    }

    private static bool BeValidCpf(string cpf)
    {
        var digits = new string(cpf.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
            return false;

        if (digits.Distinct().Count() == 1)
            return false;

        var numbers = digits.Select(c => c - '0').ToArray();

        var firstDigit = CalculateDigit(numbers, 9, 10);
        var secondDigit = CalculateDigit(numbers, 10, 11);

        return numbers[9] == firstDigit && numbers[10] == secondDigit;
    }

    private static int CalculateDigit(int[] numbers, int length, int weightStart)
    {
        var sum = 0;

        for (var index = 0; index < length; index++)
        {
            sum += numbers[index] * (weightStart - index);
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
