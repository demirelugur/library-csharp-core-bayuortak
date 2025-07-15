namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_PhoneNumberTRAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_PhoneNumberTR]
            public string Phone { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_PhoneNumberTR]
            public string? Phone { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [TestCase("5051234567")]
        [TestCase("05051234567")]
        [TestCase("905051234567")]
        [TestCase("+905051234567")]
        [TestCase("00905051234567")]
        [TestCase("(505)1234567")]
        [TestCase("(505)123--45---67")]
        [TestCase("+90(505)123--45---67")]
        public void PhoneNumberTR_ValidFormats_ShouldPass(string phone)
        {
            var model = new RequiredTestModel { Phone = phone };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [TestCase("123456789")]
        [TestCase("invalidformat")]
        [TestCase("+90505123456")]
        [TestCase("009051234567")]
        [TestCase("505 123 4567 89")]
        public void PhoneNumberTR_InvalidFormats_ShouldFail(string phone)
        {
            var model = new RequiredTestModel { Phone = phone };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void PhoneNumberTR_RequiredEmpty_ShouldFail()
        {
            var model = new RequiredTestModel { Phone = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void PhoneNumberTR_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModel { Phone = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void PhoneNumberTR_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { Phone = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void PhoneNumberTR_NullableEmpty_ShouldPass()
        {
            var model = new NullableTestModel { Phone = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}