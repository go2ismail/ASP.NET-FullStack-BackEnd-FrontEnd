// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.EncryptionManagers;


public class EncryptionService : IEncryptionService
{

    private readonly string encryptionKey = "YourEncryptionKeyvB7uZy9kL2T!gQ$wXs4mNpR@jF5xE&hC";

    public string Encrypt(string plainText)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = GenerateKey(encryptionKey);
            aesAlg.GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = GenerateKey(encryptionKey);

            byte[] iv = new byte[aesAlg.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    private static byte[] GenerateKey(string key) => SHA256.HashData(Encoding.UTF8.GetBytes(key));
}
