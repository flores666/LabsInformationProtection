namespace lib.Labs.Encryptors;

public abstract class EncryptorBase : IEncryptor
{
    public string Key { get; protected set; }
    public string ALPHABET { get; protected set; }

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

    protected Dictionary<string, string> GenerateKeyCombinations(string key)
    {
        var keys = new HashSet<string>();
        var length = key.Length;
        
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                var combination = "" + key[i] + key[j];
                keys.Add(combination);
            }
        }
        var values = keys.Shuffle();

        return keys
            .Zip(values, (key, value) => new {Key = key, Value = value})
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public bool ValidateInput(string text)
    {
        if (string.IsNullOrEmpty(ALPHABET)) return true;
        var hashAlph = new HashSet<char>(ALPHABET);
        return text.All(ch => hashAlph.Contains(ch));
    }
}