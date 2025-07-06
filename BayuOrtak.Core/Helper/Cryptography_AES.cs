namespace BayuOrtak.Core.Helper
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class Cryptography_AES
    {
        #region Private
        private const int const_key = 32;
        private const int const_iv = 16;
        private static readonly byte[] randomNumbers = new byte[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 }; // Encrypt_obfuscator işleminde oluşan şifreden bağımsız olarak belirli bir rastgele karakter kümesi oluşturur.
        private static byte[] encrypt_private(string value, Aes aes)
        {
            using (var ms = new MemoryStream())
            {
                using (var ce = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var cs = new CryptoStream(ms, ce, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(value);
                            sw.Flush();
                            cs.FlushFinalBlock();
                            return ms.ToArray();
                        }
                    }
                }
            }
        }
        private static string decrypt_private(byte[] encryptedvalue, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                using (var cd = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream(encryptedvalue))
                    {
                        using (var cs = new CryptoStream(ms, cd, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        private static byte[] generaterandomkey_private(int length)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomBytes = new byte[length];
                rng.GetBytes(randomBytes);
                return randomBytes;
            }
        }
        #endregion
        #region Default
        public static byte[] GenerateKey(string keystring, int requiredlength)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(keystring), key = new byte[requiredlength];
            Array.Copy(keyBytes, key, Math.Min(keyBytes.Length, key.Length));
            return key;
        }
        public static string Encrypt(string value, string key, string iv)
        {
            Guard.CheckEmpty(value, nameof(value));
            Guard.CheckEmpty(key, nameof(key));
            Guard.CheckEmpty(iv, nameof(iv));
            using (var aes = Aes.Create())
            {
                aes.Key = GenerateKey(key, const_key);
                aes.IV = GenerateKey(iv, const_iv);
                return Convert.ToBase64String(encrypt_private(value, aes));
            }
        }
        public static string Decrypt(string encryptedvalue, string key, string iv)
        {
            Guard.CheckEmpty(encryptedvalue, nameof(encryptedvalue));
            Guard.CheckEmpty(key, nameof(key));
            Guard.CheckEmpty(iv, nameof(iv));
            return decrypt_private(Convert.FromBase64String(encryptedvalue), GenerateKey(key, const_key), GenerateKey(iv, const_iv));
        }
        #endregion
        #region Obfuscator
        public static string Encrypt_obfuscator(string value)
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                using (var ms = new MemoryStream())
                {
                    var randomkeylength = randomNumbers[Random.Shared.Next(randomNumbers.Length)];
                    foreach (var item in new[] {
                        generaterandomkey_private(randomkeylength), // randomkeylength değeri kadar rastgele karakter üretiyor
                        aes.Key,
                        aes.IV,
                        encrypt_private(value, aes), // Veri
                        new byte[] { randomkeylength } // Baştan kaç karakterin rastgele olduğunu belirten değer
                    }) { ms.Write(item.AsSpan()); }
                    var r = Convert.ToBase64String(ms.ToArray());
                    var firstchar = r[0]; // İlk değerin char değerine göre CaesarCipherOperation ile karıştırma
                    return String.Concat(firstchar.ToString(), _other.CaesarCipherOperation(r.Substring(1), Convert.ToInt32(firstchar)));
                }
            }
        }
        public static string Decrypt_obfuscator(string encryptedvalue)
        {
            Guard.CheckEmpty(encryptedvalue, nameof(encryptedvalue));
            var firstchar = encryptedvalue[0];
            var combinedBytes = Convert.FromBase64String(String.Concat(firstchar.ToString(), _other.CaesarCipherOperation(encryptedvalue.Substring(1), -1 * Convert.ToInt32(firstchar))));
            var startindex = combinedBytes[combinedBytes.Length - 1];
            var sourceSpan = combinedBytes.AsSpan();
            var encryptedstartindex = startindex + const_key + const_iv;
            return decrypt_private(sourceSpan.Slice(encryptedstartindex, combinedBytes.Length - (1 + encryptedstartindex)).ToArray(), sourceSpan.Slice(startindex, const_key).ToArray(), sourceSpan.Slice(startindex + const_key, const_iv).ToArray());
        }
        #endregion
    }
}