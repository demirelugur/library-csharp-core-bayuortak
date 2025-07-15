namespace BayuOrtak.Core.UnitTest.Helper
{
    using NUnit.Framework;
    using System;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [TestFixture]
    public class OrtakTools_TryGoogleMapsCoordinateTests
    {
        [Test]
        public void TryGoogleMapsCoordinate_ValidCoordinate_ReturnsTrueAndUri()
        {
            var input = "40.12345,29.98765";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(uri, Is.Not.Null);
                Assert.That(uri.ToString(), Is.EqualTo("https://maps.google.com/?q=40.12345,29.98765"));
            }
        }

        [Test]
        public void TryGoogleMapsCoordinate_ValidNegativeCoordinate_ReturnsTrueAndUri()
        {
            var input = "-40.5,-29.25";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(uri, Is.Not.Null);
                Assert.That(uri.ToString(), Is.EqualTo("https://maps.google.com/?q=-40.5,-29.25"));
            }
        }

        [Test]
        public void TryGoogleMapsCoordinate_EmptyString_ReturnsFalseAndDefault()
        {
            var input = "";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(uri, Is.Null);
            }
        }

        [Test]
        public void TryGoogleMapsCoordinate_NullString_ReturnsFalseAndDefault()
        {
            string input = null;
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(uri, Is.Null);
            }
        }
        [Test]
        public void TryGoogleMapsCoordinate_OnlyOneValue_ReturnsFalseAndDefault()
        {
            var input = "40.12345";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(uri, Is.Null);
            }
        }
        [Test]
        public void TryGoogleMapsCoordinate_NonNumericValues_ReturnsFalseAndDefault()
        {
            var input = "non-numeric,invalid-value";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(uri, Is.Null);
            }
        }
        [Test]
        public void TryGoogleMapsCoordinate_OutOfRangeLatitude_ReturnsFalseAndDefault()
        {
            var input = "200,30";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(uri, Is.Null);
            }
        }
        [Test]
        public void TryGoogleMapsCoordinate_OutOfRangeLongitude_ReturnsFalseAndDefault()
        {
            var input = "40,200";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(uri, Is.Null);
            }
        }
        [Test]
        public void TryGoogleMapsCoordinate_ExtraValues_ReturnsFalseAndDefault()
        {
            var input = "40,30,20";
            var result = _try.TryGoogleMapsCoordinate(input, out Uri uri);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(uri, Is.Null);
            }
        }
    }
}