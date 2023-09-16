namespace Laba1.Encryptors;

public interface IEncryptor<T>
{
    public string Encrypt(string input);
    public string Decrypt(string input);
    public T Key { get; set; }
    public string ALPHABET { get; }
}