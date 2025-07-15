namespace BayuOrtak.Core.UnitTest.Extensions.StringExtensions
{
    using BayuOrtak.Core.Extensions;
    using NUnit.Framework;
    using System;
    [TestFixture]
    public class ReplaceSanitizeTests
    {
        [Test]
        public void ReplaceSanitize_EmptyString_IsRequiredFalse_ReturnsNull()
        {
            var input = "";
            var result = input.ReplaceSanitize(false, "test");
            Assert.That(result, Is.Null);
        }
        [Test]
        public void ReplaceSanitize_EmptyString_IsRequiredTrue_Throws()
        {
            var input = "";
            Assert.Throws<ArgumentNullException>(() => input.ReplaceSanitize(true, "test"));
        }
        [Test]
        public void ReplaceSanitize_RemovesScriptTag()
        {
            var input = "<div>test<script>alert('x')</script></div>";
            var result = input.ReplaceSanitize(false, "test");
            Assert.That(result, Does.Not.Contain("<script>"));
            Assert.That(result, Does.Contain("<div>"));
        }
        [Test]
        public void ReplaceSanitize_AllowsHttpsOnly()
        {
            var input = "<a href='http://example.com'>link</a><a href='https://example.com'>link2</a>";
            var result = input.ReplaceSanitize(false, "test");
            Assert.That(result, Does.Contain("https://example.com"));
        }
        [Test]
        public void ReplaceSanitize_AllowsDataAttributes()
        {
            var input = "<div data-test='val'>test</div>";
            var result = input.ReplaceSanitize(false, "test");
            Assert.That(result, Does.Contain("data-test"));
        }
        [Test]
        public void ReplaceSanitize_ValidHtml_RemainsIntact()
        {
            string input = "<b>bold</b>";
            var result = input.ReplaceSanitize(false, "test");
            Assert.That(result, Does.Contain("<b>"));
            Assert.That(result, Does.Contain("bold"));
        }
        [Test]
        public void ReplaceSanitize_PlainText_ReturnsSame()
        {
            var input = "plain text";
            var result = input.ReplaceSanitize(false, "test");
            Assert.That(result, Is.EqualTo("plain text"));
        }
        [Test]
        public void ReplaceSanitize_InvalidHtml_ReturnsNullOrThrows()
        {
            var input = "<div><span>";
            var result = input.ReplaceSanitize(false, "test");
            Assert.That(result, Is.EqualTo("<div><span></span></div>"));
            Assert.That(result, Is.Not.EqualTo("<div><span>"));
        }
    }
}