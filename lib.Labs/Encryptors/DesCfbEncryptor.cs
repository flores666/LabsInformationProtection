using System.Text;

namespace lib.Labs.Encryptors;

/// <summary>
/// Режим обратной связи по шифрованному блоку (CFB) включает использование предыдущего зашифрованного блока
/// для обратной связи при шифровании следующего блока данных.
/// Лаба 6
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

        var path = Path.Combine(Path.GetTempPath(), "feedback");
        _feedback = GenerateIV(blockSize);
        WriteIVIntoFile(_feedback, path);

        while (iteration_number-- > 0)
        {
            if (iteration_number == 0)
                blockSize = inputBytes.Length % blockSize;

            // Получение блока данных из входной строки
            var inputBlock = new byte[blockSize];
            int sourceIndex = inputBytes.Length - (iteration_number + 1) * blockSize;
            Array.Copy(inputBytes, sourceIndex, inputBlock, 0, Math.Min(blockSize, inputBytes.Length - sourceIndex));

            // Шифрование обратной связи
            var feedbackEncrypted = new byte[_feedback.Length];
            for (int i = 0; i < feedbackEncrypted.Length; i++)
            {
                feedbackEncrypted[i] = _des.Encrypt(_feedback[i]);
            }

            // XOR с открытым текстом
            var encryptedBlock = new byte[inputBlock.Length];
            for (int i = 0; i < encryptedBlock.Length; i++)
            {
                encryptedBlock[i] = (byte)(inputBlock[i] ^ feedbackEncrypted[i]);
            }

            // Добавление зашифрованного блока в результат
            result.AddRange(encryptedBlock);

            // Обновление обратной связи для следующей итерации
            _feedback = encryptedBlock;
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

        var path = Path.Combine(Path.GetTempPath(), "feedback");
        _feedback = GetIVFromFile(path);
        if (File.Exists(path)) File.Delete(path);

        while (iteration_number-- > 0)
        {
            if (iteration_number == 0)
                blockSize = encryptedBytes.Length % blockSize;

            // Получение блока данных из входной строки
            var encryptedBlock = new byte[blockSize];
            int sourceIndex = encryptedBytes.Length - (iteration_number + 1) * blockSize;
            Array.Copy(encryptedBytes, sourceIndex, encryptedBlock, 0,
                Math.Min(blockSize, encryptedBytes.Length - sourceIndex));

            // Шифрование обратной связи
            var feedbackEncrypted = new byte[_feedback.Length];
            for (int i = 0; i < feedbackEncrypted.Length; i++)
            {
                feedbackEncrypted[i] = _des.Encrypt(_feedback[i]);
            }

            // XOR с зашифрованным блоком данных
            var decryptedBlock = new byte[encryptedBlock.Length];
            for (int i = 0; i < decryptedBlock.Length; i++)
            {
                decryptedBlock[i] = (byte)(encryptedBlock[i] ^ feedbackEncrypted[i]);
            }

            // Добавление дешифрованного блока в результат
            result.AddRange(decryptedBlock);

            // Обновление обратной связи для следующей итерации
            _feedback = encryptedBlock;
        }

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