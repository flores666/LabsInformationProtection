using System.ComponentModel;
using System.Text;

namespace lib.Labs.Encryptors;

public class LinearEncryptor : EncryptorBase
{
    private const int B = 256;
    private readonly List<int> _sequence = new List<int>();

    public LinearEncryptor(int inputLen)
    {
        _sequence = GenerateSequence(inputLen);
    }

    public override string Encrypt(string input)
    {
        var resultBytes = new List<byte>();

        for (var i = 0; i < input.Length; i++)
        {
            var ch = input[i];
            var bytes = Encoding.UTF8.GetBytes(new char[] {ch});
            for (var j = 0; j < bytes.Length; j++)
            {
                var code = (byte) ((bytes[j] + _sequence[i]) % B);
                resultBytes.Add(code);
            }
        }

        byte[] encryptedBytes = resultBytes.ToArray();
        return Convert.ToBase64String(encryptedBytes);
    }


    public override string Decrypt(string input)
    {
        byte[] encryptedBytes = Convert.FromBase64String(input);
        var result = new StringBuilder();
        var code = new List<byte>();
        var sequenceIndex = 0;
        for (var i = 0; i < encryptedBytes.Length; i++)
        {
            code.Add((byte)(encryptedBytes[i] - _sequence[sequenceIndex]));
            
            if (code[0] < 0) code[0] = (byte)(code[0] + B);
            else if (code[0] is 208 or 209)
            {
                code.Add((byte)(encryptedBytes[++i] - _sequence[sequenceIndex]));
            } 
            
            sequenceIndex = (sequenceIndex + 1) % _sequence.Count;
            result.Append(Encoding.UTF8.GetChars(code.ToArray()));
            code.Clear();
        }

        return result.ToString();
    }

    private List<int> GenerateSequence(int length)
    {
        var A = 17;
        var C = 39;
        var result = new List<int> {41};
        var i = 1;
        while (result.Count != length)
        {
            result.Add((A * result[i - 1] + C) % B);
            i++;
        }

        return result;
    }
}
