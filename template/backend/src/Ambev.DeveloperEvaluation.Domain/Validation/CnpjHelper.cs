namespace Ambev.DeveloperEvaluation.Domain.Validation;

public static class CnpjHelper
{
    public static string Normalize(string cnpj)
    {
        return new string((cnpj ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    public static string Format(string cnpj)
    {
        var digits = Normalize(cnpj);

        if (digits.Length != 14)
            return cnpj;

        return $"{digits[..2]}.{digits.Substring(2, 3)}.{digits.Substring(5, 3)}/{digits.Substring(8, 4)}-{digits.Substring(12, 2)}";
    }

    public static bool IsValid(string cnpj)
    {
        var digits = Normalize(cnpj);

        if (digits.Length != 14)
            return false;

        if (digits.Distinct().Count() == 1)
            return false;

        var numbers = digits.Select(c => c - '0').ToArray();
        var firstDigit = CalculateDigit(numbers, 12);
        var secondDigit = CalculateDigit(numbers, 13);

        return numbers[12] == firstDigit && numbers[13] == secondDigit;
    }

    private static int CalculateDigit(int[] numbers, int length)
    {
        var weights = length == 12
            ? new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }
            : new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var sum = 0;

        for (var index = 0; index < length; index++)
        {
            sum += numbers[index] * weights[index];
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
