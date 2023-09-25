namespace lib.Labs.Encryptors;

public abstract class EncryptorBase : IEncryptor
{
    public string Key { get; protected set; }
    public string ALPHABET { get; protected set; }

    protected EncryptorBase() { }

    public static EncryptorBase GetEncryptor(LabType type, string key, string alphabet, int inputLen = 0)
    {
        switch (type)
        {
            case LabType.Lab1:
                return new SubstitutionEncryptor(key, alphabet);
            case LabType.Lab2:
                return new PermutationEncryptor(key, alphabet);
            case LabType.Lab3:
                return new LinearEncryptor(inputLen);
            default:
                return null;
        }
    }

    public abstract string Encrypt(string input);
    public abstract string Decrypt(string input);

    public bool ValidateInput(string text)
    {
        return ALPHABET == null || text.All(ch => ALPHABET.Contains(ch));
    }
}