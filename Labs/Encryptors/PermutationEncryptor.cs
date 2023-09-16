using System.Text;
using Microsoft.Extensions.Primitives;

namespace Laba1.Encryptors;

public class PermutationEncryptor : IEncryptor<int[]>
{
    public int[] Key { get; set; }
    public string ALPHABET { get; }
    private bool KeyPrepared = false;

    public PermutationEncryptor(string key)
    {
        Key = key.Split(",").Select(int.Parse).ToArray();
    }

    public string Encrypt(string input)
    {
        var result = new StringBuilder();
        var len = Key.Length;
        input = PrepareInput(input);
        for (int i = 0; i < input.Length; i += len)
        {
            var sub = input.Substring(i, len);
            var enc = new char[len];
            for (int j = 0; j < len; j++)
            {
                enc[j] = sub[Key[j]];
            }

            result.Append(enc);
        }

        return result.ToString();
    }

    private string PrepareInput(string msg)
    {
        var len = Key.Length;
        var toAddLength = len - msg.Length % len;
        var charArray = new char[toAddLength];
        for (int i = 0; i < toAddLength; i++)
        {
            charArray[i] = ' ';
        }

        return msg + new string(charArray);
    }

    public string Decrypt(string input)
    {
        var result = new StringBuilder();
        var len = Key.Length;
        Key = PrepareKey(Key);
        for (int i = 0; i < input.Length; i += len)
        {
            var sub = input.Substring(i, len);
            var dec = new char[len];
            for (int j = 0; j < len; j++)
            {
                dec[j] = sub[Key[j]];
            }

            result.Append(dec);
        }

        return result.ToString();
    }

    private int[] PrepareKey(int[] key)
    {
        var result = new int[key.Length];
        for (int i = 0; i < key.Length; i++)
        {
            result[i] = Array.IndexOf(key, i);
        }

        return result;
    }
}