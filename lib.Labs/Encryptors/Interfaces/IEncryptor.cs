namespace lib.Labs.Encryptors.Interfaces;

public interface IEncryptor
{
    public string Key { get; }
    public string ALPHABET { get; }
    public string Encrypt(string input);
    public string Decrypt(string input);
}