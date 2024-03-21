using lib.Labs.Encryptors.Interfaces;

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
            LabType.Lab4 => new DesEncryptorBase(key),
            LabType.Lab5 => new DesCbcEncryptor(key),
            LabType.Lab6 => new DesCfbEncryptor(key),
            LabType.Lab7 => new DesOfbEncryptor(key),
            LabType.Lab8 => new ChangingIntervalEncryptor(),
            LabType.Lab91 => new ChangingNumberOfSpacesEncryptor(),
            LabType.Lab92 => new ChangingNumberOfSpacesEncryptorBits(),
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