using System.Text;
using lib.Labs.Encryptors.Interfaces;

namespace lib.Labs.Encryptors;

public class ChangingIntervalEncryptor : EncryptorBase, ISteganography
{
    public string Container { get; set; }

    private const char END_MARKER = '@'; // Маркер окончания сообщения

    // Метод для предварительной обработки контейнера
    private static string PreprocessContainer(string container)
    {
        // Удаляем лишние пробелы после предложений
        container = container.Replace("  ", " ");
        // Добавляем маркер окончания сообщения
        container += END_MARKER;

        return container;
    }

    public override string Encrypt(string message)
    {
        Container = PreprocessContainer(Container);
        // Преобразование сообщения в бинарный формат
        var binaryMessage = new StringBuilder();
        foreach (char c in message)
        {
            binaryMessage.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
        }

        // Размещение сообщения в контейнере
        var stegoContainer = new StringBuilder();
        var messageIndex = 0;

        foreach (char ch in Container)
        {
            if (IsEndOfSentence(ch))
            {
                // Кодирование бита сообщения в пробелы после окончания предложения
                if (messageIndex < binaryMessage.Length)
                {
                    var numSpaces = binaryMessage[messageIndex++] == '1' ? 1 : 2;
                    stegoContainer.Append(' ', numSpaces);
                    stegoContainer.Append(ch);
                }
                else
                {
                    // Если сообщение закончилось, оставляем оригинальный пробел
                    stegoContainer.Append(ch);
                }
            }
            else
            {
                stegoContainer.Append(ch);
            }
        }

        return stegoContainer.ToString();
    }

    public override string Decrypt(string container)
    {
        var messageBuilder = new StringBuilder();

        var sentences = container.Split('.', '!', '?');

        for (int i = 0; i < sentences.Length; i++)
        {
            if (sentences[i].EndsWith("  "))
            {
                messageBuilder.Append('0');
            }
            else if (sentences[i].EndsWith(" "))
            {
                messageBuilder.Append('1');
            }

            if ((i + 1) % 8 == 0) messageBuilder.Append(" ");
        }

        var message = messageBuilder.ToString().TrimEnd();

        // Находим маркер окончания сообщения и обрезаем сообщение
        var markerIndex = message.IndexOf(END_MARKER);
        if (markerIndex != -1)
        {
            message.Remove(markerIndex, message.Length - markerIndex);
        }

        return BinaryToString(message);
    }

    private static bool IsEndOfSentence(char ch)
    {
        return ch == '.' || ch == '?' || ch == '!';
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
            var character = (char) asciiValue;
            sb.Append(character);
        }

        return sb.ToString();
    }
}