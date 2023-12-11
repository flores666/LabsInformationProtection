using lib.Labs.Encryptors.DES;

namespace lib.Labs.Encryptors;

public abstract class EncryptorBase : IEncryptor
{
    public string Key { get; protected set; }
    public string ALPHABET { get; protected set; }

    public static EncryptorBase GetEncryptor(LabType type, string key = "", string alphabet = "")
    {
        var isEmpty = string.IsNullOrEmpty(key) || string.IsNullOrEmpty(alphabet);
        return type switch
        {
            LabType.Lab1 => isEmpty ? new SubstitutionEncryptor() : new SubstitutionEncryptor(key, alphabet),
            LabType.Lab2 => isEmpty ? new PermutationEncryptor() : new PermutationEncryptor(key, alphabet),
            LabType.Lab3 => new LinearEncryptor(),
            LabType.Lab4 => new DesEncryptor(key),
            _ => null
        };
    }

    public abstract string Encrypt(string input);
    public abstract string Decrypt(string input);

    public bool ValidateInput(string text)
    {
        if (string.IsNullOrEmpty(ALPHABET)) return true;
        var hashAlph = new HashSet<char>(ALPHABET);
        return text.All(ch => hashAlph.Contains(ch));
    }
}