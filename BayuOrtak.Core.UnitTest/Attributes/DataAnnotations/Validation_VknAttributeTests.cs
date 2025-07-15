namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_VknAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_Vkn]
            public long Vkn { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_Vkn]
            public long? Vkn { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [TestCase(1234567890)]
        [TestCase(602883151)]
        public void Vkn_Valid_ShouldPass(long vkn)
        {
            var model = new RequiredTestModel { Vkn = vkn };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [TestCase(1234567891)]
        [TestCase(100000001)]
        [TestCase(10000000100)]
        public void Vkn_Invalid_ShouldFail(long vkn)
        {
            var model = new RequiredTestModel { Vkn = vkn };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void Vkn_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { Vkn = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void Vkn_NullableZero_ShouldFail()
        {
            var model = new NullableTestModel { Vkn = 0 };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void Vkn_NullableEmpty_ShouldPass()
        {
            var model = new NullableTestModel { Vkn = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}