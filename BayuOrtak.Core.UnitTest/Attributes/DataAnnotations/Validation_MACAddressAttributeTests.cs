namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_MACAddressAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_MACAddress]
            public string MAC { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_MACAddress]
            public string? MAC { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void MACAddress_ValidFormat_ShouldPass()
        {
            var model = new RequiredTestModel { MAC = "01:23:45:67:89:AB" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MACAddress_ValidFormatWithDash_ShouldPass()
        {
            var model = new RequiredTestModel { MAC = "01-23-45-67-89-AB" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MACAddress_LowerCase_ShouldPass()
        {
            var model = new RequiredTestModel { MAC = "aa:bb:cc:dd:ee:ff" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MACAddress_InvalidFormat_ShouldFail()
        {
            var model = new RequiredTestModel { MAC = "01:23:45:67:89" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MACAddress_InvalidCharacters_ShouldFail()
        {
            var model = new RequiredTestModel { MAC = "01:23:45:67:89:ZZ" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MACAddress_RequiredEmpty_ShouldFail()
        {
            var model = new RequiredTestModel { MAC = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MACAddress_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModel { MAC = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MACAddress_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { MAC = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MACAddress_NullableEmpty_ShouldPass()
        {
            var model = new NullableTestModel { MAC = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}