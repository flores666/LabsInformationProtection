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
        var sequenceIndex = 0;
        for (var i = 0; i < input.Length; i++)
        {
            var ch = input[i];
            var bytes = Encoding.UTF8.GetBytes(new char[] {ch});
            for (var j = 0; j < bytes.Length; j++)
            {
                var code = (byte) ((bytes[j] + _sequence[sequenceIndex++]) % B);
                resultBytes.Add(code);
            }
        }

        return Convert.ToBase64String(resultBytes.ToArray());
    }


    public override string Decrypt(string input)
    {
        byte[] encryptedBytes = Convert.FromBase64String(input);
        var codes = new List<byte>();

        for (var i = 0; i < encryptedBytes.Length; i++)
        {
            codes.Add((byte) (encryptedBytes[i] - _sequence[i]));
            if (codes[i] < 0) codes[i] = (byte) (codes[i] + B);
        }

        return Encoding.UTF8.GetString(codes.ToArray());
    }

    private List<int> GenerateSequence(int length)
    {
        var A = 17;
        var C = 39;
        var result = new List<int>(capacity:length*2) {41};
        var i = 1;
        var maxBytesInSymbol = 4;
        while (result.Count != length * maxBytesInSymbol)
        {
            result.Add((A * result[i - 1] + C) % B);
            i++;
        }

        return result;
    }
}