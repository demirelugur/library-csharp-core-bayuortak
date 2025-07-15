namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using System.ComponentModel.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    [TestFixture]
    public class Validation_BayburtUniSicilNoAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_BayburtUniSicilNo]
            public string SicilNo { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_BayburtUniSicilNo]
            public string? SicilNo { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void SicilNo_Valid4Digit_ShouldPass()
        {
            var model = new RequiredTestModel { SicilNo = "1234" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void SicilNo_ValidIleBaslayan_ShouldPass()
        {
            var model = new RequiredTestModel { SicilNo = "İ-12" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void SicilNo_InvalidFormat_ShouldFail()
        {
            var model = new RequiredTestModel { SicilNo = "12A4" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void SicilNo_TooLong_ShouldFail()
        {
            var model = new RequiredTestModel { SicilNo = "12345" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void SicilNo_ZeroOrNegative_ShouldFail()
        {
            var model = new RequiredTestModel { SicilNo = "0" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void SicilNo_RequiredEmpty_ShouldFail()
        {
            var model = new RequiredTestModel { SicilNo = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void SicilNo_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModel { SicilNo = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void SicilNo_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { SicilNo = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void SicilNo_NullableEmpty_ShouldPass()
        {
            var model = new NullableTestModel { SicilNo = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}