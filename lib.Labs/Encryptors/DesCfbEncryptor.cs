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

        var path = Path.Combine(Path.GetTempPath(), "feedback");
        _feedback = GenerateIV(blockSize);
        WriteIVIntoFile(_feedback, path);
        
        while (iteration_number-- > 0)
        {
            if (iteration_number == 0)
                blockSize = inputBytes.Length % blockSize;

            // Получение блока данных из входной строки
            var encBlock = new byte[blockSize];
            int sourceIndex = inputBytes.Length - (iteration_number + 1) * blockSize;
            Array.Copy(inputBytes,
                sourceIndex,
                encBlock,
                0,
                Math.Min(blockSize, inputBytes.Length - sourceIndex));


            // Шифрование обратной связи
            var feedbackEncrypted = new byte[_feedback.Length];
            for (int i = 0; i < feedbackEncrypted.Length; i++)
            {
                feedbackEncrypted[i] = _des.Encrypt(_feedback[i]);
            }

            // XOR с зашифрованным блоком данных
            var decryptedBlock = new byte[encBlock.Length];
            for (int i = 0; i < decryptedBlock.Length; i++)
            {
                decryptedBlock[i] = (byte)(encBlock[i] ^ feedbackEncrypted[i]);
                result.Add(decryptedBlock[i]);
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

        var path = Path.Combine(Path.GetTempPath(), "feedback");
        _feedback = GetIVFromFile(path);
        if (File.Exists(path)) File.Delete(path);
        
        while (iteration_number-- > 0)
        {
            if (iteration_number == 0)
                blockSize = inputBytes.Length % blockSize;

            // Получение блока данных из входной строки
            var encBlock = new byte[blockSize];
            var sourceIndex = inputBytes.Length - (iteration_number + 1) * blockSize;
            Array.Copy(inputBytes, sourceIndex, encBlock, 0, blockSize);

            // Шифрование обратной связи
            var feedbackEncrypted = new byte[_feedback.Length];
            for (int i = 0; i < feedbackEncrypted.Length; i++)
            {
                feedbackEncrypted[i] = _des.Encrypt(_feedback[i]);
            }

            // XOR с зашифрованным блоком данных
            var decryptedBlock = new byte[encBlock.Length];
            for (int i = 0; i < decryptedBlock.Length; i++)
            {
                decryptedBlock[i] = (byte)(encBlock[i] ^ feedbackEncrypted[i]);
                result.Add(decryptedBlock[i]);
            }

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