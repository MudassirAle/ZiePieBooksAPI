using System.Security.Cryptography;

namespace ZiePieBooksAPI.Helper
{
    public static class EncryptionHelper
    {
        private const int Iterations = 10000;
        private const int KeySize = 256; // Key size in bits
        private const int IVSize = 128; // IV size in bits

        public static string Encrypt(string plainText, string passphrase)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] salt = GenerateRandomBytes(16); // Salt for PBKDF2

                // Derive key and IV from passphrase using PBKDF2
                using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(passphrase, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    aesAlg.Key = keyDerivationFunction.GetBytes(KeySize / 8);
                    aesAlg.IV = keyDerivationFunction.GetBytes(IVSize / 8);
                }

                // Perform encryption
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] cipherTextBytes;
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    cipherTextBytes = msEncrypt.ToArray();
                }

                // Concatenate salt with cipher text for storage
                byte[] resultBytes = new byte[salt.Length + cipherTextBytes.Length];
                Array.Copy(salt, 0, resultBytes, 0, salt.Length);
                Array.Copy(cipherTextBytes, 0, resultBytes, salt.Length, cipherTextBytes.Length);

                return Convert.ToBase64String(resultBytes);
            }
        }

        public static string Decrypt(string cipherText, string passphrase)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // Extract salt from cipher text
            byte[] salt = new byte[16];
            Array.Copy(cipherTextBytes, 0, salt, 0, salt.Length);

            // Derive key and IV from passphrase using PBKDF2
            using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(passphrase, salt, Iterations, HashAlgorithmName.SHA256))
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyDerivationFunction.GetBytes(KeySize / 8);
                    aesAlg.IV = keyDerivationFunction.GetBytes(IVSize / 8);

                    // Perform decryption
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    string plainText;
                    using (var msDecrypt = new MemoryStream(cipherTextBytes, salt.Length, cipherTextBytes.Length - salt.Length))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plainText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                    return plainText;
                }
            }
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }

    //public static class PassphraseEncryption
    //{
    //    // Define constants for key derivation
    //    private const int KeySize = 256; // Key size in bits
    //    private const int BlockSize = 128; // Block size in bits
    //    private const int SaltSize = 16; // Salt size in bytes
    //    private const int Iterations = 10000; // Number of iterations for the key derivation

    //    public static string EncryptString(string plainText, string passphrase)
    //    {
    //        if (string.IsNullOrEmpty(plainText))
    //            throw new ArgumentNullException(nameof(plainText));

    //        if (string.IsNullOrEmpty(passphrase))
    //            throw new ArgumentNullException(nameof(passphrase));

    //        using (var aesAlg = Aes.Create())
    //        {
    //            var salt = GenerateSalt(SaltSize);
    //            var key = DeriveKeyFromPassphrase(passphrase, salt);

    //            aesAlg.Key = key;
    //            aesAlg.GenerateIV();

    //            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

    //            using (var msEncrypt = new MemoryStream())
    //            {
    //                // Prepend the salt and IV to the ciphertext
    //                msEncrypt.Write(salt, 0, salt.Length);
    //                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

    //                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
    //                {
    //                    using (var swEncrypt = new StreamWriter(csEncrypt))
    //                    {
    //                        swEncrypt.Write(plainText);
    //                    }
    //                }

    //                return Convert.ToBase64String(msEncrypt.ToArray());
    //            }
    //        }
    //    }

    //    public static string DecryptString(string cipherText, string passphrase)
    //    {
    //        if (string.IsNullOrEmpty(cipherText))
    //            throw new ArgumentNullException(nameof(cipherText));

    //        if (string.IsNullOrEmpty(passphrase))
    //            throw new ArgumentNullException(nameof(passphrase));

    //        var fullCipher = Convert.FromBase64String(cipherText);

    //        using (var aesAlg = Aes.Create())
    //        {
    //            using (var msDecrypt = new MemoryStream(fullCipher))
    //            {
    //                var salt = new byte[SaltSize];
    //                msDecrypt.Read(salt, 0, salt.Length);

    //                var iv = new byte[aesAlg.BlockSize / 8];
    //                msDecrypt.Read(iv, 0, iv.Length);

    //                var key = DeriveKeyFromPassphrase(passphrase, salt);

    //                aesAlg.Key = key;
    //                aesAlg.IV = iv;

    //                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

    //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
    //                {
    //                    using (var srDecrypt = new StreamReader(csDecrypt))
    //                    {
    //                        return srDecrypt.ReadToEnd();
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    private static byte[] GenerateSalt(int size)
    //    {
    //        var salt = new byte[size];
    //        using (var rng = new RNGCryptoServiceProvider())
    //        {
    //            rng.GetBytes(salt);
    //        }
    //        return salt;
    //    }

    //    private static byte[] DeriveKeyFromPassphrase(string passphrase, byte[] salt)
    //    {
    //        using (var keyDerivationFunction = new Rfc2898DeriveBytes(passphrase, salt, Iterations))
    //        {
    //            return keyDerivationFunction.GetBytes(KeySize / 8);
    //        }
    //    }
    //}

    //public static class EncryptionHelper
    //{
    //    //private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourEncryptionKey"); // Change this to your own key
    //    //private static readonly byte[] IV = Encoding.UTF8.GetBytes("YourEncryptionIV"); // Change this to your own IV

    //    private static readonly byte[] Key = new byte[]
    //    {
    //        0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
    //        0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
    //    }; // 128-bit key for AES encryption

    //    private static readonly byte[] IV = new byte[]
    //    {
    //        0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
    //        0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
    //    }; // 128-bit IV for AES encryption

    //    public static string Encrypt(string plainText)
    //    {
    //        using (Aes aesAlg = Aes.Create())
    //        {
    //            aesAlg.Key = Key;
    //            aesAlg.IV = IV;

    //            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

    //            using (MemoryStream msEncrypt = new MemoryStream())
    //            {
    //                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
    //                {
    //                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
    //                    {
    //                        swEncrypt.Write(plainText);
    //                    }
    //                }
    //                return Convert.ToBase64String(msEncrypt.ToArray());
    //            }
    //        }
    //    }

    //    public static string Decrypt(string cipherText)
    //    {
    //        using (Aes aesAlg = Aes.Create())
    //        {
    //            aesAlg.Key = Key;
    //            aesAlg.IV = IV;

    //            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

    //            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
    //            {
    //                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
    //                {
    //                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
    //                    {
    //                        return srDecrypt.ReadToEnd();
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
