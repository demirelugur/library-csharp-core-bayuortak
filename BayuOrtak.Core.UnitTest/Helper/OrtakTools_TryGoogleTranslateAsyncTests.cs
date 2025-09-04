namespace BayuOrtak.Core.UnitTest.Helper
{
    using BayuOrtak.Core.Extensions;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [TestFixture]
    public class OrtakTools_TryGoogleTranslateAsyncTests
    {
        [Test]
        public async Task TryGoogleTranslateAsync_ValidTranslation_ReturnsTranslatedText()
        {
            var input = "merhaba dünya";
            var timeout = TimeSpan.FromSeconds(10);
            var cancellationtoken = CancellationToken.None;
            var (statuswarning, value) = await _try.TryGoogleTranslateAsync(input, timeout, cancellationtoken, to: "en", from: "tr");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(statuswarning, Is.False);
                Assert.That(value, Is.Not.Empty);
            }
            Assert.That(value.ToSeoFriendly(), Is.EqualTo("hello-world"));
        }
        [Test]
        public async Task TryGoogleTranslateAsync_EmptyString_ReturnsEmpty()
        {
            var input = "";
            var timeout = TimeSpan.FromSeconds(10);
            var cancellationtoken = CancellationToken.None;
            var (statuswarning, value) = await _try.TryGoogleTranslateAsync(input, timeout, cancellationtoken, to: "en", from: "tr");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(statuswarning, Is.False);
                Assert.That(value, Is.Empty);
            }
        }
        [Test]
        public async Task TryGoogleTranslateAsync_NullString_ReturnsEmpty()
        {
            string input = null;
            var timeout = TimeSpan.FromSeconds(10);
            var cancellationtoken = CancellationToken.None;
            var (statuswarning, value) = await _try.TryGoogleTranslateAsync(input, timeout, cancellationtoken, to: "en", from: "tr");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(statuswarning, Is.False);
                Assert.That(value, Is.Empty);
            }
        }
    }
}