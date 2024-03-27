using System.Text;
using lib.Labs.Encryptors.Interfaces;

namespace lib.Labs.Encryptors;

public class ChangingNumberOfSpacesEncryptorBits : EncryptorBase, ISteganographyBits
{
    private const char END_MARKER = '@'; // Маркер окончания сообщения

    public string Container { get; set; }
    public int BitsNumber { get; set; }

    public override string Encrypt(string input)
    {
        if (!LengthEnough(Container, (input.Length * 16) / BitsNumber))
        {
            return "Не хватает длины контейнера, увеличьте его";
        }

        // Преобразование сообщения в бинарный формат
        var binaryMessage = new StringBuilder();
        foreach (char c in input)
        {
            binaryMessage.Append(Convert.ToString(c, 2).PadLeft(16, '0'));
        }

        PreprocessContainer(binaryMessage.Length);

        // Размещение сообщения в контейнере
        var stegoContainer = new StringBuilder();
        var binaryMessageIndex = 0;
        var totalSpacesCount = 0;
        
        foreach (char ch in Container)
        {
            if (ch == '\n')
            {
                for (int i = 0; binaryMessageIndex < binaryMessage.Length && i < BitsNumber; i++)
                {
                    var insertChar = binaryMessage[binaryMessageIndex++] == '1' ? (char)160 : ' ';
                    stegoContainer.Append(insertChar);
                    totalSpacesCount++;
                }
            }
            
            stegoContainer.Append(ch);

            if (binaryMessageIndex == binaryMessage.Length)
            {
                stegoContainer.Append(END_MARKER);
                stegoContainer.Append(Container.AsSpan(stegoContainer.Length - totalSpacesCount - 1, 
                    Container.Length - (stegoContainer.Length - totalSpacesCount) - 1));
                break;
            }
        }

        return stegoContainer.ToString();
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

    /*public override string Decrypt(string input)
    {
        var encodedBits = new StringBuilder();
        var index = 0;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == END_MARKER) break;
            if (input[i] == '\n')
            {
                for (int j = 1; j <= BitsNumber; j++)
                {
                    if (input[i - j] == ' ') encodedBits.Append('0');
                    else encodedBits.Append('1');
                    if (++index % 16 == 0) encodedBits.Append(' ');
                }
            }
        }

        return BinaryToString(encodedBits.ToString().TrimEnd());
    }*/

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

    private void PreprocessContainer(int rowsToProcess)
    {
        var rowIndex = 0;

        for (var i = 0; rowIndex < rowsToProcess; i++)
        {
            if (Container[i] != '\n') continue;

            var j = i;

            while (Container[j] == ' ' || Container[j] == (char)160)
            {
                j--;
            }

            Container = Container.Remove(j, i - j);

            rowIndex++;
        }
    }

    private string BinaryToString(string binary)
    {
        var sb = new StringBuilder();

        var binaryValues = binary.Split(' ');
        foreach (string binaryValue in binaryValues)
        {
            // Преобразование двоичной строки в целое число
            var asciiValue = Convert.ToInt32(binaryValue, 2);

            // Преобразование ASCII-кода в символ и добавление его к результату
            var character = (char)asciiValue;
            sb.Append(character);
        }

        return sb.ToString();
    }
}