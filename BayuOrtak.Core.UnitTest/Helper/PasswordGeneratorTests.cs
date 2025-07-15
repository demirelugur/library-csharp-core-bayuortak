namespace BayuOrtak.Core.UnitTest.Helper
{
    using BayuOrtak.Core.Helper;
    using NUnit.Framework;
    using System.Linq;
    [TestFixture]
    public class PasswordGeneratorTests
    {
        [Test]
        public void Generate_CustomGenerator_OnlyDigitsAndLowercase()
        {
            var gen = new PasswordGenerator("UDF", "abc", "123", ";|.");
            var password = gen.Generate();
            Assert.That(password, Has.Length.InRange(8, 16));
            Assert.That(password.All(c => "UDFabc123;|.".Contains(c)), Is.True);
        }
        [Test]
        public void GenerateRandomChars_ReturnsCorrectLengthAndCharset()
        {
            var chars = PasswordGenerator.GenerateRandomChars(10, "xyz");
            Assert.That(chars, Has.Length.EqualTo(10));
            Assert.That(chars.All(c => "xyz".Contains(c)), Is.True);
        }
        [Test]
        public void IsStrongPassword_ValidPassword_ReturnsTrue()
        {
            var strong = "Abc1!xyz";
            Assert.That(PasswordGenerator.IsStrongPassword(strong), Is.True);
        }
        [TestCase("abc1!xyz", 8)]
        [TestCase("ABC1!XYZ", 8)]
        [TestCase("Abcdefgh!", 8)]
        [TestCase("Abcdefg1", 8)]
        [TestCase("A1!a", 8)]
        public void IsStrongPassword_InvalidCases_ReturnsFalse(string password, int minLen)
        {
            Assert.That(PasswordGenerator.IsStrongPassword(password, minLen), Is.False);
        }
    }
} 