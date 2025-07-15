namespace BayuOrtak.Core.Helper.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class AESHelper
    {
        #region Private
        private const int key_requiredlength = 32;
        private const int iv_requiredlength = 16;
        private static readonly byte[] randomnumbers = new byte[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 }; // ObfuscatorEncrypt işleminde oluşan şifreden bağımsız olarak belirli bir rastgele karakter kümesi oluşturur.
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
                var _randombytes = new byte[length];
                rng.GetBytes(_randombytes);
                return _randombytes;
            }
        }
        private static byte[] generatekey_private(string keystring, int requiredlength)
        {
            byte[] keybytes = Encoding.UTF8.GetBytes(keystring), key = new byte[requiredlength];
            Array.Copy(keybytes, key, Math.Min(keybytes.Length, key.Length));
            return key;
        }
        #endregion
        public static string Encrypt(string value, string key, string iv)
        {
            Guard.CheckEmpty(value, nameof(value));
            Guard.CheckEmpty(key, nameof(key));
            Guard.CheckEmpty(iv, nameof(iv));
            using (var aes = Aes.Create())
            {
                aes.Key = generatekey_private(key, key_requiredlength);
                aes.IV = generatekey_private(iv, iv_requiredlength);
                return Convert.ToBase64String(encrypt_private(value, aes));
            }
        }
        public static string Decrypt(string encryptedvalue, string key, string iv)
        {
            Guard.CheckEmpty(encryptedvalue, nameof(encryptedvalue));
            Guard.CheckEmpty(key, nameof(key));
            Guard.CheckEmpty(iv, nameof(iv));
            return decrypt_private(Convert.FromBase64String(encryptedvalue), generatekey_private(key, key_requiredlength), generatekey_private(iv, iv_requiredlength));
        }
        public static string ObfuscatorEncrypt(string value)
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                using (var ms = new MemoryStream())
                {
                    var _randomkeylength = randomnumbers[Random.Shared.Next(randomnumbers.Length)];
                    foreach (var item in new[] {
                        generaterandomkey_private(_randomkeylength), // randomkeylength değeri kadar rastgele karakter üretiyor
                        aes.Key,
                        aes.IV,
                        encrypt_private(value, aes), // Veri
                        new byte[] { _randomkeylength } // Baştan kaç karakterin rastgele olduğunu belirten değer
                    }) { ms.Write(item.AsSpan()); }
                    var _r = Convert.ToBase64String(ms.ToArray());
                    var _firstchar = _r[0]; // İlk değerin char değerine göre CaesarCipherOperation ile karıştırma
                    return String.Concat(_firstchar.ToString(), _other.CaesarCipherOperation(_r.Substring(1), Convert.ToInt32(_firstchar)));
                }
            }
        }
        public static string ObfuscatorDecrypt(string encryptedvalue)
        {
            Guard.CheckEmpty(encryptedvalue, nameof(encryptedvalue));
            var _firstchar = encryptedvalue[0];
            var _combinedbytes = Convert.FromBase64String(String.Concat(_firstchar.ToString(), _other.CaesarCipherOperation(encryptedvalue.Substring(1), -1 * Convert.ToInt32(_firstchar))));
            var _startindex = _combinedbytes[_combinedbytes.Length - 1];
            var _sourcespan = _combinedbytes.AsSpan();
            var _encryptedstartindex = _startindex + key_requiredlength + iv_requiredlength;
            return decrypt_private(_sourcespan.Slice(_encryptedstartindex, _combinedbytes.Length - (1 + _encryptedstartindex)).ToArray(), _sourcespan.Slice(_startindex, key_requiredlength).ToArray(), _sourcespan.Slice(_startindex + key_requiredlength, iv_requiredlength).ToArray());
        }
    }
}