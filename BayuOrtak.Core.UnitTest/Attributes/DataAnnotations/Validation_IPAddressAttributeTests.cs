namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_IPAddressAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_IPAddress]
            public string IP { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_IPAddress]
            public string? IP { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void IPAddress_ValidIPv4_ShouldPass()
        {
            var model = new RequiredTestModel { IP = "192.168.1.1" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void IPAddress_ValidIPv4WithLeadingZeros_ShouldPass()
        {
            var model = new RequiredTestModel { IP = "010.000.000.001" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void IPAddress_InvalidFormat_ShouldFail()
        {
            var model = new RequiredTestModel { IP = "999.999.999.999" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void IPAddress_InvalidString_ShouldFail()
        {
            var model = new RequiredTestModel { IP = "not-an-ip" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void IPAddress_RequiredEmpty_ShouldFail()
        {
            var model = new RequiredTestModel { IP = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void IPAddress_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModel { IP = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void IPAddress_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { IP = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void IPAddress_NullableEmpty_ShouldPass()
        {
            var model = new NullableTestModel { IP = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}