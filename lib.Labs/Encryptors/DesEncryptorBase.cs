using System.Text;
using lib.Labs.Encryptors.Interfaces;

namespace lib.Labs.Encryptors;

/// <summary>
/// Симметричный алгоритм блочного шифрования.
/// Лаб 4
/// </summary>
public class DesEncryptorBase : EncryptorBase, IKeyGenerative
{
    protected readonly DesAlgorithm _des;

    public DesEncryptorBase(string key)
    {
        _des = new DesAlgorithm(key);
    }

    public override string Encrypt(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var result = new List<byte>();

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
            int sourceIndex = inputBytes.Length - (iteration_number + 1) * blockSize;
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
                result.Add(output[i]);
            }
        }

        return Convert.ToBase64String(result.ToArray());
    }


    public override string Decrypt(string input)
    {
        var inputBytes = Convert.FromBase64String(input);
        var result = new List<byte>();
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
            var sourceIndex = inputBytes.Length - (iteration_number + 1) * blockSize;
            Array.Copy(inputBytes, sourceIndex, inputBlock, 0, blockSize);

            // Дешифрование блока данных
            var output = new byte[inputBlock.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = _des.Decrypt(inputBlock[i]);
                result.Add(output[i]);
            }
        }

        return Encoding.UTF8.GetString(result.ToArray());
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