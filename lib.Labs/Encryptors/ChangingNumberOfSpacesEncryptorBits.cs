using System.Text;
using lib.Labs.Encryptors.Interfaces;

namespace lib.Labs.Encryptors;

public class ChangingNumberOfSpacesEncryptorBits : EncryptorBase, ISteganographyBits
{
    public string Container { get; set; }
    public int BitsNumber { get; set; }

    public override string Encrypt(string input)
    {
        var result = new StringBuilder(Container.Length + input.Length * 2);

        int k = 0;
        for (int i = 0, j = 0; i < input.Length; k++)
        {
            if (Container[k] == '\n')
            {
                for (int z = 0; i < input.Length && z < BitsNumber; z++)
                {
                    bool isZeroBit = (input[i] & ((char)1 << 15 - j)) == 0;
                    if (isZeroBit)
                        result.Append(' ');
                    else
                        result.Append((char)160); // ascii Non-breaking space

                    j++;

                    if (j == 16)
                    {
                        j = 0;
                        i++;
                    }
                }
            }

            result.Append(Container[k]);
        }

        result.Append(Container[k]);
        k++;
        result.Append(Container.AsSpan(k..));

        return result.ToString();
    }

    public override string Decrypt(string input)
    {
        List<byte> encodedBits = new(16);

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '\n' && (input[i - 1] == ' ' || input[i - 1] == (char)160))
            {
                var toAdd = new Stack<byte>(BitsNumber);

                for (int j = 0; j < BitsNumber; j++)
                {
                    if (input[(i - 1) - j] == ' ')
                        toAdd.Push(0);
                    else
                        toAdd.Push(1);
                }

                while (toAdd.Count != 0)
                    encodedBits.Add(toAdd.Pop());
            }
        }

        int charsCount = encodedBits.Count / 16;

        // encodedBits.Count + encodedBits.Count % 2 - since each char is 2 bytes 
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
}