namespace BayuOrtak.Core.UnitTest.Extensions.StringExtensions
{
    using BayuOrtak.Core.Extensions;
    using NUnit.Framework;
    [TestFixture]
    public class ToTitleCaseTests
    {
        [Test]
        public void ToTitleCase_EmptyString_ReturnsEmpty()
        {
            var result = "".ToTitleCase(true, null);
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public void ToTitleCase_SingleChar_ReturnsUpper()
        {
            var result = "a".ToTitleCase(true, null);
            Assert.That(result, Is.EqualTo("A"));
        }
        [Test]
        public void ToTitleCase_WhitespaceOnly_ReturnsEmpty()
        {
            var result = "   ".ToTitleCase(true, null);
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public void ToTitleCase_DefaultPunctuations_TitleCaseApplied()
        {
            var result = "hello world this is a test".ToTitleCase(true, default);
            Assert.That(result, Is.EqualTo("Hello World This Ýs A Test"));
        }
        [Test]
        public void ToTitleCase_CustomPunctuations_TitleCaseApplied()
        {
            var result = "test-case/with:punctuations".ToTitleCase(true, new[] { '-', '/', ':' });
            Assert.That(result, Is.EqualTo("Test-Case/With:Punctuations"));
        }
        [Test]
        public void ToTitleCase_WhitespaceFalse_OnlyPunctuationsSplitWords()
        {
            var result = "hello world.test-case".ToTitleCase(false, new[] { '.', '-' });
            Assert.That(result, Is.EqualTo("Hello world.Test-Case"));
        }
        [Test]
        public void ToTitleCase_MixedCaseInput_NormalizesToTitleCase()
        {
            var result = "hELLo wORld".ToTitleCase(true, null);
            Assert.That(result, Is.EqualTo("Hello World"));
        }
    }
}