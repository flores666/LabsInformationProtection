using lib.Labs.Encryptors.Interfaces;

namespace lib.Labs.Encryptors;

public class ChangingNumberOfSpacesEncryptor : EncryptorBase, ISteganography
{
    public string Container { get; set; }

    public override string Encrypt(string input)
    {
        throw new NotImplementedException();
    }

    public override string Decrypt(string input)
    {
        throw new NotImplementedException();
    }

}