using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    private static readonly string EncryptionKey = "YourStrongKeyHere@123"; // Keep it secret & constant

    public static string Encrypt(string plainText)
    {
        byte[] clearBytes = Encoding.UTF8.GetBytes(plainText);
        using (Aes encryptor = Aes.Create())
        {
            var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x53, 0x47, 0x19, 0x37, 0x72, 0x55, 0x10, 0x27 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                return Convert.ToBase64String(ms.ToArray())
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", ""); // URL-safe
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        cipherText = cipherText.Replace("-", "+").Replace("_", "/");
        switch (cipherText.Length % 4)
        {
            case 2: cipherText += "=="; break;
            case 3: cipherText += "="; break;
        }

        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x53, 0x47, 0x19, 0x37, 0x72, 0x55, 0x10, 0x27 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
