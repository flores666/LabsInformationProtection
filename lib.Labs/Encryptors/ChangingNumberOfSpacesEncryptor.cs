using System.Text;
using lib.Labs.Encryptors.Interfaces;

namespace lib.Labs.Encryptors;

public class ChangingNumberOfSpacesEncryptor : EncryptorBase, ISteganography
{
    private const char END_MARKER = '@'; // Маркер окончания сообщения
    public string Container { get; set; }

    public override string Encrypt(string input)
    {
        if (!LengthEnough(Container, input.Length * 16))
        {
            return "Не хватает длины контейнера, увеличьте его";
        }

        // Преобразование сообщения в бинарный формат
        var binaryMessage = new StringBuilder();
        foreach (char c in input)
        {
            binaryMessage.Append(Convert.ToString(c, 2).PadLeft(16, '0'));
        }

        // Размещение сообщения в контейнере
        var stegoContainer = new StringBuilder();
        var binaryMessageIndex = 0;
        var symbolIndex = 0;

        foreach (char ch in Container)
        {
            if (ch == '\n')
            {
                var numSpaces = binaryMessage[binaryMessageIndex++] == '1' ? 1 : 2;
                stegoContainer.Append(' ', numSpaces);
                stegoContainer.Append(ch);
                
                if (binaryMessageIndex == binaryMessage.Length)
                {
                    stegoContainer.Append(END_MARKER);
                    stegoContainer.Append(Container.AsSpan(symbolIndex, Container.Length - symbolIndex));
                    break;
                }
            }
            else
            {
                stegoContainer.Append(ch);
            }

            symbolIndex++;
        }

        return stegoContainer.ToString();
    }

    public override string Decrypt(string input)
    {
        var encodedBits = new StringBuilder();
        var index = 0;
        
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == END_MARKER) break;
            if (input[i] == '\n')
            {
                if (i >= 2 && input[i - 2] == ' ') encodedBits.Append('0');
                else encodedBits.Append('1');
                if (++index % 16 == 0) encodedBits.Append(' ');
            }
        }
        
        return BinaryToString(encodedBits.ToString().TrimEnd());
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