using System.Diagnostics;
using System.Text;

namespace Labs.Encryptors;

//Lab 3
public class LinearEncryptor : EncryptorBase
{
    //constants to generate random numbers
    private const int A = 17;
    private const int B = 256;
    private const int C = 39;
    private const int _generativeNumber = 41;

    public override string Encrypt(string input)
    {
        var resultBytes = new List<byte>();
        var sequenceIndex = 0;
        var prevNum = _generativeNumber;

        for (var i = 0; i < input.Length; i++)
        {
            var ch = input[i];
            var bytes = Encoding.UTF8.GetBytes(new char[] {ch});
            for (var j = 0; j < bytes.Length; j++)
            {
                var rand = (A * prevNum + C) % B;
                var code = (byte) ((bytes[j] + rand) % B);
                prevNum = rand;
                resultBytes.Add(code);
            }
        }

        return Convert.ToBase64String(resultBytes.ToArray());
    }


    public override string Decrypt(string input)
    {
        byte[] encryptedBytes = Convert.FromBase64String(input);
        var codes = new List<byte>();
        var prevNum = _generativeNumber;
        for (var i = 0; i < encryptedBytes.Length; i++)
        {
            var rand = (A * prevNum + C) % B;
            codes.Add((byte) (encryptedBytes[i] - rand));
            prevNum = rand;
            if (codes[i] < 0) codes[i] = (byte) (codes[i] + B);
        }

        return Encoding.UTF8.GetString(codes.ToArray());
    }
}