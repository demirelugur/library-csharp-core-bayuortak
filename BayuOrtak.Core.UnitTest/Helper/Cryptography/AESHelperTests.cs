namespace BayuOrtak.Core.UnitTest.Helper.Cryptography
{
    using BayuOrtak.Core.Helper.Cryptography;
    using NUnit.Framework;
    using System;
    [TestFixture]
    public class AESHelperTests
    {
        private const string Key = "12345678901234567890123456789012";
        private const string IV = "1234567890123456";
        [Test]
        public void EncryptDecrypt_Roundtrip_ReturnsOriginal()
        {
            var original = "Test message!";
            var encrypted = AESHelper.Encrypt(original, Key, IV);
            var decrypted = AESHelper.Decrypt(encrypted, Key, IV);
            Assert.That(decrypted, Is.EqualTo(original));
        }
        [Test]
        public void ObfuscatorEncryptDecrypt_Roundtrip_ReturnsOriginal()
        {
            var original = "Obfuscated message!";
            var encrypted = AESHelper.ObfuscatorEncrypt(original);
            var decrypted = AESHelper.ObfuscatorDecrypt(encrypted);
            Assert.That(decrypted, Is.EqualTo(original));
        }
        [Test]
        public void EncryptDecrypt_LongText_Roundtrip()
        {
            var original = new String('A', 10000);
            var encrypted = AESHelper.Encrypt(original, Key, IV);
            var decrypted = AESHelper.Decrypt(encrypted, Key, IV);
            Assert.That(decrypted, Is.EqualTo(original));
        }
    }
}