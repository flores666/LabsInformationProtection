using System.Text;
using lib.Labs.Encryptors.Interfaces;

namespace lib.Labs.Encryptors;

public class ChangingNumberOfSpacesEncryptor : EncryptorBase, ISteganography
{
    public string Container { get; set; }

    public override string Encrypt(string input)
    {
        if (!LengthEnough(Container, input.Length * 16))
        {
            return "Не хватает длины контейнера, увеличьте его";
        }
        
        var result = new StringBuilder(Container.Length + input.Length * 2);

        int k = 0;
        for (int i = 0, j = 0; i < input.Length;)
        {
            if (Container[k] == '\n')
            {
                bool isZeroBit = (input[i] & ((char)1 << 15 - j)) == 0;
                if (isZeroBit) result.Append("  ");
                else result.Append(' ');

                j++;

                if (j == 16)
                {
                    j = 0;
                    i++;
                }
            }

            result.Append(Container[k]);
            k++;
        }

        result.Append(Container[k..]);

        return result.ToString();
    }

    public override string Decrypt(string input)
    {
        var encodedBits = new List<byte>(16);

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '\n')
            {
                // Проверяем, что индекс не выходит за пределы массива
                if (i >= 2 && input[i - 2] == ' ') encodedBits.Add(0);
                else encodedBits.Add(1);
            }
        }

        int charsCount = encodedBits.Count / 16;

        var encodedChars = new char[charsCount];

        for (int i = 0; i < charsCount; i++)
        {
            for (int j = i * 16; j < (i + 1) * 16; j++)
            {
                encodedChars[i] |= (char)((char)encodedBits[j] << 15 - (j - i * 16));
            }
        }

        return new string(encodedChars);
    }
    
    private bool LengthEnough(string container, int inputLength)
    {
        var counter = 0;
        foreach (var c in container)
        {
            if (c == '\n') counter++;
            if (inputLength == counter) return true;
        }

        return false;
    }
}