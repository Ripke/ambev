namespace Ambev.DeveloperEvaluation.Common.Security;

public interface ICpfProtectionService
{
    string Normalize(string cpf);
    string Encrypt(string cpf);
    string Decrypt(string encryptedCpf);
    string Mask(string cpf);
}
