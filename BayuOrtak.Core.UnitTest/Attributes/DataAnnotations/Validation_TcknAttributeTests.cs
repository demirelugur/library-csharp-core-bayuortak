namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_TcknAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_Tckn]
            public long Tckn { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_Tckn]
            public long? Tckn { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [TestCase(10000000146)]
        [TestCase(11111111110)]
        public void Tckn_Valid_ShouldPass(long tckn)
        {
            var model = new RequiredTestModel { Tckn = tckn };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [TestCase(0)]
        [TestCase(12345678901)]
        [TestCase(-1234567890)]
        public void Tckn_Invalid_ShouldFail(long tckn)
        {
            var model = new RequiredTestModel { Tckn = tckn };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void Tckn_NullableZero_ShouldFail()
        {
            var model = new NullableTestModel { Tckn = 0 };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void Tckn_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { Tckn = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}