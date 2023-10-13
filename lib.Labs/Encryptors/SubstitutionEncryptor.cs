using System.Text;

namespace lib.Labs.Encryptors;

//Lab 1
public class SubstitutionEncryptor : EncryptorBase
{
    private readonly Dictionary<string, string> _keyCombinations;

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

        _keyCombinations = GenerateKeyCombinations(Key);
    }

    public override string Encrypt(string text)
    {
        var splitString = SplitString(text, 2);

        var result = new StringBuilder();
        foreach (var chunk in splitString)
        {
            if (chunk.Length == 2) result.Append(_keyCombinations[chunk]);
            else result.Append(_keyCombinations[chunk + "0"]);
        }

        return string.Join("", result);
    }

    public override string Decrypt(string text)
    {
        var splitString = SplitString(text, 2);
        var result = new StringBuilder();

        foreach (var chunk in splitString)
        {
            result.Append(_keyCombinations.FirstOrDefault(v => v.Value == chunk).Key);
        }

        return result.ToString();
    }

    private static List<string> SplitString(string input, int chunkSize)
    {
        int length = input.Length;
        int numOfChunks = (length + chunkSize - 1) / chunkSize;

        var chunks = new List<string>();

        for (int i = 0; i < numOfChunks; i++)
        {
            int startIndex = i * chunkSize;
            int endIndex = Math.Min(startIndex + chunkSize, length);
            chunks.Add(input.Substring(startIndex, endIndex - startIndex));
        }

        return chunks;
    }
}