namespace lib.Labs.Encryptors;

public interface IEncryptor
{
    public string Encrypt(string input);
    public string Decrypt(string input);
    public string Key { get; }
    public string ALPHABET { get; }
}