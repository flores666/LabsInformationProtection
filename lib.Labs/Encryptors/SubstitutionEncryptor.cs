using System.Text;
using lib.Labs.Encryptors.Interfaces;
using lib.Labs.Extensions;

namespace lib.Labs.Encryptors;

//Lab 1
public class SubstitutionEncryptor : EncryptorBase, IKeyGenerative
{
    private readonly Dictionary<string, string> _keyCombinations;

    public SubstitutionEncryptor()
    {
    }

    public SubstitutionEncryptor(string key, string alphabet)
    {
        ALPHABET = alphabet;
        Key = "042513";
        var length = ALPHABET.Length;

        if (key.Length < length)
        {
            Key = new string((key + Key).Distinct().ToArray());
        }
        else if (key.Length == length)
        {
            Key = key;
        }

        var el = ConvertToDictionary(key, ']', ',');
        _keyCombinations = el.Count == 0 ? GenerateKeyCombinations(Key) : el;
    }

    public override string Encrypt(string text)
    {
        if (text.Length % 2 == 1) text += '0';

        var result = new StringBuilder(text.Length);

        for (int i = 0; i < text.Length; i += 2)
        {
            var chunk = new string(new char[] { text[i], text[i + 1] });
            result.Append(_keyCombinations[chunk]);
        }

        return result.ToString();
    }

    public override string Decrypt(string text)
    {
        if (text.Length % 2 == 1) text += '0';

        var newDic = _keyCombinations
            .ToDictionary(p => p.Value, p => p.Key);
        var result = new StringBuilder(text.Length);

        for (int i = 0; i < text.Length; i += 2)
        {
            var chunk = new string(new char[] { text[i], text[i + 1] });
            result.Append(newDic[chunk]);
        }

        return result.ToString();
    }

    public string GenerateKey()
    {
        const string nums = "012345";
        var rand = new Random();
        var key = new HashSet<char>();

        while (key.Count != nums.Length)
        {
            var ch = nums[rand.Next(nums.Length)];
            key.Add(ch);
        }

        return string.Join("", GenerateKeyCombinations(string.Join("", key)));
    }

    private static Dictionary<string, string> GenerateKeyCombinations(string key)
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
            .Zip(values, (key, value) => new { Key = key, Value = value })
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private static Dictionary<string, string> ConvertToDictionary(string input, char pairDelimiter,
        char keyValueDelimiter)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        string[] pairs = input.Split(pairDelimiter, StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split(keyValueDelimiter);

            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim('[');
                string value = keyValue[1].Trim(']');
                dictionary[key] = value;
            }
        }

        return dictionary;
    }
}