namespace lib.Labs.Encryptors;

public abstract class EncryptorBase : IEncryptor
{
    public string Key { get; protected set; }
    public string ALPHABET { get; protected set; }

    protected EncryptorBase() { }

    public static EncryptorBase GetEncryptor(LabType type, string key, string alphabet)
    {
        switch (type)
        {
            case LabType.Lab1:
                return new SubstitutionEncryptor(key, alphabet);
            case LabType.Lab2:
                return new PermutationEncryptor(key, alphabet);
            case LabType.Lab3:
                return new LinearEncryptor();
            default:
                return null;
        }
    }

    public abstract string Encrypt(string input);
    public abstract string Decrypt(string input);
    
    public List<string> GenerateKeyCombinations(string input)
    {
        var result = new HashSet<string>();
        var length = input.Length;

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                var combination = "" + input[i] + input[j];
                result.Add(combination);
            }
        }

        return result.ToList();
    }

    public bool ValidateInput(string text)
    {
        if (string.IsNullOrEmpty(ALPHABET)) return true;
        var hashAlph = new HashSet<char>(ALPHABET);
        return text.All(ch => hashAlph.Contains(ch));
    }
}