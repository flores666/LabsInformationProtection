namespace lib.Labs.Encryptors;

public class SubstitutionEncryptor : EncryptorBase
{
    private readonly List<string> _keyCombinations;
    
    public SubstitutionEncryptor(string key, string alphabet)
    {
        ALPHABET = alphabet;
        Key = "042513";
        var length = ALPHABET.Length;
        _keyCombinations = GenerateCombinations(Key);
        
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
        var splittedString = SplitString(text, 2);
        
        var result = new List<string>();
        foreach (var chunk in splittedString)
        {
            var i = int.Parse(chunk[0].ToString());
            if (chunk.Length == 2)
            {
                var j = int.Parse(chunk[1].ToString());
                result.Add(_keyCombinations[i] + _keyCombinations[j]);
            }
            else
            {
                result.Add(_keyCombinations[i]);
            }
        }

        return string.Join("", result);
    }

    public override string Decrypt(string text)
    {
        var splittedString = SplitString(text, 2);
        var result = new List<string>();
        
        foreach (var chunk in splittedString)
        {
            result.Add(_keyCombinations.IndexOf(chunk).ToString());
        }

        return string.Join("", result);
    }

    private static List<string> GenerateCombinations(string input)
    {
        var result = new List<string>();
        var length = input.Length;

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                var combination = "" + input[i] + input[j];
                if (!result.Contains(combination))
                {
                    result.Add(combination);
                }
            }
        }

        return result;
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