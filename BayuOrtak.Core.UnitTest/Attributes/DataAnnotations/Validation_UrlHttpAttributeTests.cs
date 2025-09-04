namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_UrlHttpAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_UrlHttp]
            public string Url { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_UrlHttp]
            public string? Url { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [TestCase("http://example.com")]
        [TestCase("https://example.com/path?query=1")]
        [TestCase("https://www.bayburt.edu.tr")]
        public void UrlHttp_Valid_ShouldPass(string url)
        {
            var model = new RequiredTestModel { Url = url };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [TestCase("ftp://example.com")]
        [TestCase("file:///C:/test.txt")]
        [TestCase("mailto:test@example.com")]
        [TestCase("ws://example.com")]
        [TestCase("wss://example.com")]
        [TestCase("not-a-url")]
        [TestCase("://missing.scheme.com")]
        public void UrlHttp_Invalid_ShouldFail(string url)
        {
            var model = new RequiredTestModel { Url = url };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void UrlHttp_RequiredEmpty_ShouldFail()
        {
            var model = new RequiredTestModel { Url = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void UrlHttp_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModel { Url = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void UrlHttp_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { Url = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void UrlHttp_NullableEmpty_ShouldPass()
        {
            var model = new NullableTestModel { Url = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}