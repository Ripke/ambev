using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Ambev.DeveloperEvaluation.Common.Security;

public class AesCpfProtectionService : ICpfProtectionService
{
    private const int IvSize = 16;
    private readonly byte[] _key;

    public AesCpfProtectionService(IConfiguration configuration)
    {
        var keyMaterial = configuration["CpfProtection:Key"];

        if (string.IsNullOrWhiteSpace(keyMaterial))
        {
            keyMaterial = configuration["Jwt:SecretKey"];
        }

        if (string.IsNullOrWhiteSpace(keyMaterial))
            throw new InvalidOperationException("No key configured for CPF protection.");

        _key = SHA256.HashData(Encoding.UTF8.GetBytes(keyMaterial));
    }

    public string Normalize(string cpf)
    {
        var digits = new string(cpf.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
            throw new ArgumentException("CPF must contain 11 digits.", nameof(cpf));

        return digits;
    }

    public string Encrypt(string cpf)
    {
        var normalizedCpf = Normalize(cpf);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();

        memoryStream.Write(aes.IV, 0, aes.IV.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
        {
            writer.Write(normalizedCpf);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public string Decrypt(string encryptedCpf)
    {
        var payload = Convert.FromBase64String(encryptedCpf);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = payload.Take(IvSize).ToArray();
        var cipher = payload.Skip(IvSize).ToArray();

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        using var memoryStream = new MemoryStream(cipher);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream, Encoding.UTF8);

        var cpf = reader.ReadToEnd();
        return Normalize(cpf);
    }

    public string Mask(string cpf)
    {
        var normalizedCpf = Normalize(cpf);
        return $"{normalizedCpf[..3]}.xxx.xxx-{normalizedCpf[^2..]}";
    }
}
