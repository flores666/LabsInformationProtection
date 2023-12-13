using System.Text;
using lib.Labs.Encryptors.DES;

namespace lib.Labs.Encryptors;

public class DesCbcEncryptor : DesEncryptorBase
{
    private byte[] _iv;

    public DesCbcEncryptor(string key) : base(key)
    {
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

        // Задаем IV перед началом цикла
        _iv = GenerateIV(blockSize);

        while (iteration_number-- > 0)
        {
            if (iteration_number == 0)
                blockSize = inputBytes.Length % blockSize;

            var inputBlock = new byte[blockSize];
            Array.Copy(inputBytes,
                inputBytes.Length - (iteration_number + 1) * blockSize,
                inputBlock,
                0,
                blockSize);

            // XOR с предыдущим зашифрованным блоком (или IV для первого блока)
            for (int i = 0; i < blockSize; i++)
            {
                inputBlock[i] ^= _iv[i];
            }

            // Шифрование блока данных
            var output = new byte[inputBlock.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = _des.Encrypt(inputBlock[i]);
                result.Add(output[i]);
            }

            // Сохраняем текущий зашифрованный блок в IV для следующей итерации
            _iv = output;
        }

        return Convert.ToBase64String(result.ToArray());
    }

    public override string Decrypt(string input)
    {
        var encryptedBytes = Convert.FromBase64String(input);
        var result = new List<byte>();

        var blockSize = 4 * 1024;
        int iteration_number;

        if (encryptedBytes.Length < blockSize)
            iteration_number = 1;
        else if (encryptedBytes.Length % blockSize == 0)
            iteration_number = encryptedBytes.Length / blockSize;
        else
            iteration_number = (encryptedBytes.Length / blockSize) + 1;

        // Задаем IV перед началом цикла
        _iv = GenerateIV(blockSize);

        while (iteration_number-- > 0)
        {
            if (iteration_number == 0)
                blockSize = encryptedBytes.Length % blockSize;

            var encryptedBlock = new byte[blockSize];
            Array.Copy(encryptedBytes,
                encryptedBytes.Length - (iteration_number + 1) * blockSize,
                encryptedBlock,
                0,
                blockSize);

            // Расшифрование блока данных
            var decryptedBlock = new byte[encryptedBlock.Length];
            for (int i = 0; i < decryptedBlock.Length; i++)
            {
                decryptedBlock[i] = _des.Decrypt(encryptedBlock[i]);
                // XOR с предыдущим зашифрованным блоком (или IV для первого блока)
                decryptedBlock[i] ^= _iv[i];
                result.Add(decryptedBlock[i]);
            }

            // Сохраняем текущий расшифрованный блок в IV для следующей итерации
            _iv = encryptedBlock;
        }

        // Преобразуем результат в строку UTF-8
        return Encoding.UTF8.GetString(result.ToArray());
    }


    /// <summary>
    /// "вектор инициализации" (IV) для следующего блока данных. 
    /// </summary>
    /// <param name="blockSize">размер блока</param>
    /// <returns></returns>
    private byte[] GenerateIV(int blockSize)
    {
        // Генерируем случайный IV длиной в blockSize байт
        var iv = new byte[blockSize];
        // Здесь нужно использовать надежный механизм генерации случайных чисел
        // Например, можно воспользоваться классом RNGCryptoServiceProvider
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(iv);
        }

        return iv;
    }
}