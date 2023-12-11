using System.Collections;
using System.Text;

namespace lib.Labs.Encryptors.DES;

public class DesEncryptor : EncryptorBase, IKeyGenerative
{
    private readonly DesAlgorithm _des;

    public DesEncryptor(string key)
    {
        _des = new DesAlgorithm(key);
    }

    public override string Encrypt(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] result = null;
        // Создание потока для записи результата
        var blockSize = 4 * 1024;
        int iteration_number;

        if (inputBytes.Length < blockSize)
            iteration_number = 1;
        else if (inputBytes.Length % blockSize == 0)
            iteration_number = inputBytes.Length / blockSize;
        else
            iteration_number = (inputBytes.Length / blockSize) + 1;

        while (iteration_number-- > 0)
        {
            if (iteration_number == 0)
                blockSize = inputBytes.Length % blockSize;

            // Получение блока данных из входной строки
            var inputBlock = new byte[blockSize];
            int sourceIndex = Math.Max(0, inputBytes.Length - (iteration_number + 1) * blockSize);
            Array.Copy(inputBytes,
                sourceIndex,
                inputBlock,
                0,
                Math.Min(blockSize, inputBytes.Length - sourceIndex));


            // Шифрование блока данных
            var output = new byte[inputBlock.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = _des.Encrypt(inputBlock[i]);
            }

            result = output;
        }

        // Возвращение зашифрованной строки в виде Base64
        return Convert.ToBase64String(result);
    }
    

    public override string Decrypt(string input)
    {
        // Преобразование входной строки из формата Base64 в массив байт
        byte[] inputBytes = Convert.FromBase64String(input);

        // Создание потока для записи результата
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bwr = new BinaryWriter(ms))
            {
                int blocksize = 4 * 1024;
                int iteration_number;

                if (inputBytes.Length < blocksize)
                    iteration_number = 1;
                else if (inputBytes.Length % blocksize == 0)
                    iteration_number = inputBytes.Length / blocksize;
                else
                    iteration_number = (inputBytes.Length / blocksize) + 1;

                while (iteration_number-- > 0)
                {
                    if (iteration_number == 0)
                        blocksize = inputBytes.Length % blocksize;

                    // Получение блока данных из входной строки
                    byte[] inputBlock = new byte[blocksize];
                    Array.Copy(inputBytes, inputBytes.Length - (iteration_number + 1) * blocksize, inputBlock, 0, blocksize);

                    // Дешифрование блока данных
                    byte[] output = new byte[inputBlock.Length];
                    for (int i = 0; i < output.Length; i++)
                    {
                        output[i] = _des.Decrypt(inputBlock[i]);
                    }

                    // Запись дешифрованного блока в результат
                    bwr.Write(output);
                    bwr.Flush();
                }

                // Возвращение дешифрованной строки в виде UTF-8
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }

    
    public string GenerateKey()
    {
        const int len = 10;
        var binaryArray = new bool[len];
        var random = new Random();

        for (int i = 0; i < len; i++)
        {
            binaryArray[i] = random.Next(2) == 1;
        }

        return string.Join("", binaryArray.Select(b => b ? "1" : "0"));
    }
}