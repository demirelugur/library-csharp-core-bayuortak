namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_BayburtUniEMailAddressAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_BayburtUniEMailAddress]
            public string email { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_BayburtUniEMailAddress]
            public string? email { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void BayburtUniEMailAddress_ValidBayburtUniEmail_ShouldPass()
        {
            var model = new RequiredTestModel { email = "test@bayburt.edu.tr" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void BayburtUniEMailAddress_InvalidEmailFormat_ShouldFail()
        {
            var model = new RequiredTestModel { email = "invalid-email-format" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEMailAddress_NonBayburtUniEmail_ShouldFail()
        {
            var model = new RequiredTestModel { email = "test@gmail.com" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEMailAddress_RequiredEmptyEmail_ShouldFail()
        {
            var model = new RequiredTestModel { email = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEMailAddress_RequiredNullEmail_ShouldFail()
        {
            var model = new RequiredTestModel { email = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEMailAddress_NullableNullEmail_ShouldPass()
        {
            var model = new NullableTestModel { email = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void BayburtUniEMailAddress_NullableEmptyEmail_ShouldPass()
        {
            var model = new NullableTestModel { email = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}