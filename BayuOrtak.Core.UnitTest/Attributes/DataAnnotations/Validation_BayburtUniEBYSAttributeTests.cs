namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_BayburtUniEBYSAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_BayburtUniEBYS]
            public string ebyscode { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_BayburtUniEBYS]
            public string? ebyscode { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void BayburtUniEBYS_Required_ShouldPass()
        {
            var model = new RequiredTestModel { ebyscode = "12.07.2025-12345" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void BayburtUniEBYS_InvalidFormatString_ShouldFailed()
        {
            var model = new RequiredTestModel { ebyscode = "123456-12.07.2025" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEBYS_NonEBYSFormattedString_ShouldFailed()
        {
            var model = new RequiredTestModel { ebyscode = "invalid-ebyscode" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEBYS_NotExactlyTwoParts_ShouldFailed()
        {
            var model = new RequiredTestModel { ebyscode = "12.07.2025-123-45" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEBYS_DateBeforeJune2008_ShouldFailed()
        {
            var model = new RequiredTestModel { ebyscode = "31.05.2008-12345" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEBYS_MoreThan16StringLenght_ShouldFailed()
        {
            var model = new RequiredTestModel { ebyscode = "12.07.2025-123456" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEBYS_RequiredStringNull_ShouldFailed()
        {
            var model = new RequiredTestModel { ebyscode = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEBYS_RequiredStringEmpty_ShouldFailed()
        {
            var model = new RequiredTestModel { ebyscode = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void BayburtUniEBYS_NullableStringNull_ShouldPass()
        {
            var model = new NullableTestModel { ebyscode = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void BayburtUniEBYS_NullableStringEmpty_ShouldPass()
        {
            var model = new NullableTestModel { ebyscode = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}