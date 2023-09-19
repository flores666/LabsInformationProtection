using System.Text;

namespace lib.Labs.Encryptors;

public class PermutationEncryptor : EncryptorBase
{
    private int[] _key;
    
    public PermutationEncryptor(string key)
    {
        Key = key;
        _key = Key.Split(",").Select(int.Parse).ToArray();
    }

    public override string Encrypt(string input)
    {
        if (!ValidateInput(input)) return "error";
        var result = new StringBuilder();
        var len = _key.Length;
        input = AddSpacesToEnd(input);
        
        for (int i = 0; i < input.Length; i += len)
        {
            var sub = input.Substring(i, len);
            var enc = new char[len];
            for (int j = 0; j < len; j++)
            {
                enc[j] = sub[_key[j]];
            }

            result.Append(enc);
        }

        return result.ToString();
    }

    private string AddSpacesToEnd(string msg)
    {
        var len = _key.Length;
        var toAddLength = len - msg.Length % len;
        if (toAddLength == 0) return msg;
        
        var spaces = new char[toAddLength];
        for (int i = 0; i < toAddLength; i++)
        {
            spaces[i] = ' ';
        }

        return msg + new string(spaces);
    }

    public override string Decrypt(string input)
    {
        if (!ValidateInput(input)) return "error";
        var result = new StringBuilder();
        var len = _key.Length;
        _key = PrepareKeyToDecrypt();
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

    private int[] PrepareKeyToDecrypt()
    {
        var result = new int[_key.Length];
        for (int i = 0; i < _key.Length; i++)
        {
            result[i] = Array.IndexOf(_key, i);
        }

        return result;
    }
}