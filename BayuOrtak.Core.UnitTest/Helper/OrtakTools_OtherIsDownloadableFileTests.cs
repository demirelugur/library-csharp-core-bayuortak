namespace BayuOrtak.Core.UnitTest.Helper
{
    using NUnit.Framework;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [TestFixture]
    public class OrtakTools_OtherIsDownloadableFileTests
    {
        [TestCase("file.pdf", false)]
        [TestCase("file.PDF", false)]
        [TestCase("file.jpg", false)]
        [TestCase("file.jpeg", false)]
        [TestCase("file.png", false)]
        [TestCase("file.gif", false)]
        [TestCase("file.bmp", false)]
        [TestCase("file.tiff", false)]
        [TestCase("file.svg", false)]
        [TestCase("", false)]
        [TestCase("   ", false)]
        [TestCase("file", true)]
        [TestCase("file.txt", true)]
        [TestCase("file.docx", true)]
        [TestCase("file.zip", true)]
        [TestCase("file.mp3", true)]
        [TestCase("file.unknownext", true)]
        public void IsDownloadableFile_VariousInputs_ReturnsExpected(string path, bool expected)
        {
            var result = _other.IsDownloadableFile(path);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}