using System.Text;

namespace lib.Labs.Encryptors;

public class LinearEncryptor : EncryptorBase
{
    private const int B = 256;
    private List<int> _sequence = new List<int>();

    public override string Encrypt(string input)
    {
        _sequence = GenerateSequence(input.Length);
        var result = "";

        for (var i = 0; i < _sequence.Count; i++)
        {
            var code = (input[i] + _sequence[i]) % B;
            result += new string(Convert.ToChar(code), 1);
        }

        return result;
    }


    public override string Decrypt(string input)
    {
        var result = "";

        for (var i = 0; i < _sequence.Count; i++)
        {
            var code = (input[i] - _sequence[i]) % B;
            if (code < 0) code += B;
            result += new string(Convert.ToChar(code), 1);
        }

        return result;
    }

    private List<int> GenerateSequence(int length)
    {
        var A = 17;
        var C = 39;
        var result = new List<int> { 41 };
        var i = 1;
        while (result.Count != length)
        {
            result.Add((A * result[i - 1] + C) % B);
            i++;
        }

        return result;
    }
}