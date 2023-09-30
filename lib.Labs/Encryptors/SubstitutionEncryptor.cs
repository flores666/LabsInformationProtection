using System.Text;

namespace lib.Labs.Encryptors;

//Lab1
public class SubstitutionEncryptor : EncryptorBase
{
    private readonly List<string> _keyCombinations;

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
        else
        {
            Key = new string(key.Substring(0, length).Distinct().ToArray());
        }
        _keyCombinations = GenerateKeyCombinations(Key);
    }

    public override string Encrypt(string text)
    {
        var splitString = SplitString(text, 2);

        var result = new StringBuilder();
        foreach (var chunk in splitString)
        {
            var i = int.Parse(chunk[0].ToString());
            if (chunk.Length == 2)
            {
                var j = int.Parse(chunk[1].ToString());
                result.Append(_keyCombinations[i] + _keyCombinations[j]);
            }
            else
            {
                result.Append(_keyCombinations[i]);
            }
        }

        return string.Join("", result);
    }

    public override string Decrypt(string text)
    {
        var splitString = SplitString(text, 2);
        var result = new StringBuilder();

        foreach (var chunk in splitString)
        {
            result.Append(_keyCombinations.IndexOf(chunk));
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