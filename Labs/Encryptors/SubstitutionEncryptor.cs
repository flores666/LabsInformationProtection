namespace Laba1.Encryptors;

public class SubstitutionEncryptor : IEncryptor<string>
{
    public string ALPHABET { get; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    public string Key { get; set; }

    public SubstitutionEncryptor(string key)
    {
        Key = "мфтйбвкёгцрсуэихьноажлпеяюызщшчъд";
        var length = ALPHABET.Length;
        if (key.Length < length)
        {
            Key = new string((key + Key).Distinct().ToArray());
        }
        else if (key.Length == length)
        {
            Key = key;
        }
        else
        {
            Key = key.Substring(0, length);
        }
    }
    
    public string Encrypt(string text)
    {
        var result = new List<char>();
        foreach (var ch in text)
        {
            var index = ALPHABET.IndexOf(ch);
            if (index == -1)
            {
                result.Add(ch);
            } else result.Add(Key[index]);
        }

        return new string(result.ToArray());
    }

    public string Decrypt(string text)
    {
        var result = new List<char>();
        foreach (var ch in text)
        {
            var index = Key.IndexOf(ch);
            if (index == -1)
            {
                result.Add(ch);
            } else result.Add(ALPHABET[index]);
        }

        return new string(result.ToArray());
    }

}