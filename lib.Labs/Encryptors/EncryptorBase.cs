namespace lib.Labs.Encryptors;

public abstract class EncryptorBase : IEncryptor
{
    public string Key { get; protected set; }
    public string ALPHABET { get; protected set; }

    protected EncryptorBase() { }

    public static EncryptorBase GetEncryptor(LabType type, string key)
    {
        switch (type)
        {
            case LabType.Lab1:
                return new SubstitutionEncryptor(key);
            case LabType.Lab2:
                return new PermutationEncryptor(key);
            default:
                return null;
        }
    }

    public abstract string Encrypt(string input);
    public abstract string Decrypt(string input);

    protected bool ValidateInput(string text)
    {
        return text.All(ch => ALPHABET.Contains(ch));
    }
}