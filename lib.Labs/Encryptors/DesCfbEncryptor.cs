using System.Text;

namespace lib.Labs.Encryptors;

/// <summary>
/// Режим обратной связи по шифрованному блоку (CFB) включает использование предыдущего зашифрованного блока
/// для обратной связи при шифровании следующего блока данных. 
/// </summary>
public class DesCfbEncryptor : DesEncryptorBase
{
    private byte[] _feedback;

    public DesCfbEncryptor(string key) : base(key)
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

        // Задаем feedback перед началом цикла
        var path = Path.Combine(Path.GetTempPath(), "feedback");
        _feedback = GenerateIV(blockSize);
        WriteIVIntoFile(_feedback, path);

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

            // Шифрование обратной связи
            var feedbackEncrypted = new byte[_feedback.Length];
            for (int i = 0; i < feedbackEncrypted.Length; i++)
            {
                feedbackEncrypted[i] = _des.Encrypt(_feedback[i]);
            }

            // XOR с блоком данных
            for (int i = 0; i < blockSize; i++)
            {
                inputBlock[i] ^= feedbackEncrypted[i];
            }

            // Шифрование блока данных
            var encryptedBlock = new byte[inputBlock.Length];
            for (int i = 0; i < encryptedBlock.Length; i++)
            {
                encryptedBlock[i] = _des.Encrypt(inputBlock[i]);
            }

            // Сохраняем текущий зашифрованный блок в feedback для следующей итерации
            _feedback = encryptedBlock;

            // Объединяем результаты текущей итерации
            result = result == null ? encryptedBlock.ToList() : result.Concat(encryptedBlock).ToList();
        }

        return Convert.ToBase64String(result.ToArray());
    }

    public override string Decrypt(string input)
    {
        var encryptedBytes = Encoding.UTF8.GetBytes(input);
        byte[] result = null;

        var blockSize = 4 * 1024;
        int iteration_number;

        if (encryptedBytes.Length < blockSize)
            iteration_number = 1;
        else if (encryptedBytes.Length % blockSize == 0)
            iteration_number = encryptedBytes.Length / blockSize;
        else
            iteration_number = (encryptedBytes.Length / blockSize) + 1;

        // Задаем feedback перед началом цикла
        var path = Path.Combine(Path.GetTempPath(), "feedback");
        _feedback = GetIVFromFile(path);
        if(File.Exists(path)) File.Delete(path);

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

            // Шифрование обратной связи
            var feedbackEncrypted = new byte[_feedback.Length];
            for (int i = 0; i < feedbackEncrypted.Length; i++)
            {
                feedbackEncrypted[i] = _des.Encrypt(_feedback[i]);
            }

            // XOR с зашифрованным блоком данных
            for (int i = 0; i < blockSize; i++)
            {
                encryptedBlock[i] ^= feedbackEncrypted[i];
            }

            // Шифрование предыдущего зашифрованного блока (или feedback)
            var decryptedBlock = new byte[encryptedBlock.Length];
            for (int i = 0; i < decryptedBlock.Length; i++)
            {
                decryptedBlock[i] = _des.Encrypt(encryptedBlock[i]);
            }

            // Сохраняем текущий зашифрованный блок в feedback для следующей итерации
            _feedback = encryptedBlock;

            // Объединяем результаты текущей итерации
            result = result == null ? decryptedBlock : result.Concat(decryptedBlock).ToArray();
        }

        // Преобразуем результат в строку UTF-8
        return Convert.ToBase64String(result);
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
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(iv);
        }

        return iv;
    }

    private byte[] GetIVFromFile(string path)
    {
        using var sr = new StreamReader(path);
        var res = sr.ReadToEnd().Split(' ').Select(s => s != "" ? byte.Parse(s) : default).ToArray();
        sr.Close();
        return res;
    }

    private void WriteIVIntoFile(byte[] iv, string path)
    {
        using var sw = new StreamWriter(path);
        foreach (var b in iv)
        {
            sw.Write(b + " ");
        }

        sw.Close();
    }
}