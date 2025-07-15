namespace BayuOrtak.Core.UnitTest.Helper
{
    using NUnit.Framework;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [TestFixture]
    public class OrtakTools_OtherIsHtmlTests
    {
        [TestCase("", false)]
        [TestCase("   ", false)]
        [TestCase("1 < 2 and 3 > 2", false)]
        [TestCase("plain text", false)]
        [TestCase("<b>bold</b>", true)]
        [TestCase("<div class='test'>content</div>", true)]
        [TestCase("<img src='x.png' />", true)]
        [TestCase("<a href='url'>link</a>", true)]
        [TestCase("<notatag>test</notatag>", true)]
        [TestCase("<br>", true)]
        [TestCase("<input type='text' value='val' />", true)]
        public void IsHtml_VariousInputs_ReturnsExpected(string input, bool expected)
        {
            var result = _other.IsHtml(input);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}