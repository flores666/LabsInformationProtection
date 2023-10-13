using System.Text;

namespace lib.Labs.Encryptors;

//Lab 2
public class PermutationEncryptor : EncryptorBase
{
    private int[] _key;

    public PermutationEncryptor(string key, string alphabet)
    {
        Key = key;
        _key = Key.Split(",").Select(int.Parse).ToArray();
        ALPHABET = alphabet;
    }

    public override string Encrypt(string input)
    {
        var result = new StringBuilder();
        var keyLength = _key.Length;
        input = FillEnding(input);

        for (int i = 0; i < input.Length; i += keyLength)
        {
            var sub = input.Substring(i, keyLength);
            var enc = new char[keyLength];
            for (int j = 0; j < keyLength; j++)
            {
                enc[j] = sub[_key[j]];
            }

            result.Append(enc);
        }

        return result.ToString();
    }

    public override string Decrypt(string input)
    {
        var result = new StringBuilder();
        var len = _key.Length;
        _key = PrepareKey();
        for (int i = 0; i < input.Length; i += len)
        {
            var sub = input.Substring(i, len);
            var dec = new char[len];
            for (int j = 0; j < len; j++)
            {
                dec[j] = sub[_key[j]];
            }

            result.Append(dec);
        }

        return result.ToString();
    }

    private string FillEnding(string msg)
    {
        var len = _key.Length;
        var toAddLength = len - msg.Length % len;
        if (toAddLength == 0) return msg;

        var spaces = new char[toAddLength];
        for (int i = 0; i < toAddLength; i++)
        {
            spaces[i] = ALPHABET[0];
        }

        return msg + new string(spaces);
    }

    private int[] PrepareKey()
    {
        var result = new int[_key.Length];
        for (int i = 0; i < _key.Length; i++)
        {
            result[i] = Array.IndexOf(_key, i);
        }

        return result;
    }
}