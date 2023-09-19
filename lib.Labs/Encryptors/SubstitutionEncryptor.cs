namespace lib.Labs.Encryptors;

public class SubstitutionEncryptor : EncryptorBase
{
    public SubstitutionEncryptor(string key)
    {
        ALPHABET = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
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
    
    public override string Encrypt(string text)
    {
        if (!ValidateInput(text)) return "error";
        var result = new List<char>();
        foreach (var ch in text)
        {
            var index = ALPHABET.IndexOf(ch);
            result.Add(index == -1 ? ch : Key[index]);
        }

        return new string(result.ToArray());
    }

    public override string Decrypt(string text)
    {
        if (!ValidateInput(text)) return "error";
        var result = new List<char>();
        foreach (var ch in text)
        {
            var index = Key.IndexOf(ch);
            result.Add(index == -1 ? ch : ALPHABET[index]);
        }

        return new string(result.ToArray());
    }

}