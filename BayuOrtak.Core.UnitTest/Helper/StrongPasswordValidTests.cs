namespace BayuOrtak.Core.UnitTest.Helper
{
    using BayuOrtak.Core.Helper;
    using NUnit.Framework;
    [TestFixture]
    public class StrongPasswordValidTests
    {
        [Test]
        public void TryIsWarning_ValidPassword_ReturnsNoWarnings()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("Abc1!xyzQ", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.False);
        }
        [Test]
        public void TryIsWarning_TooShortPassword_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("A1!a", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
        [Test]
        public void TryIsWarning_TooLongPassword_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 10, true, true, true);
            var result = validator.TryIsWarning("Abc1!xyzQwerty", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
        [Test]
        public void TryIsWarning_ConsecutiveDigits_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("Abc123!x", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
        [Test]
        public void TryIsWarning_ContainsSpace_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("Abc1! x y", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
        [Test]
        public void TryIsWarning_ContainsTurkishChar_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("Abc1!şxyz", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
        [Test]
        public void TryIsWarning_ContainsBirthYear_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("Abc1!1990x", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
        [Test]
        public void TryIsWarning_ContainsName_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("Abc1!aliX", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
        [Test]
        public void TryIsWarning_ContainsSurName_ReturnsWarning()
        {
            var validator = new StrongPasswordValid(8, 16, true, true, true);
            var result = validator.TryIsWarning("Abc1!veliX", 1990, "Ali", "Veli", "tr", out _);
            Assert.That(result, Is.True);
        }
    }
}