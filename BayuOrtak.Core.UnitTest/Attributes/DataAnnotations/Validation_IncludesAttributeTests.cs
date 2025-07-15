namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using BayuOrtak.Core.Enums;
    using System.ComponentModel.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    [TestFixture]
    public class Validation_IncludesAttributeTests
    {
        // String için test modelleri
        private class StringRequiredModel
        {
            [Validation_Required]
            [Validation_IncludesAttribute<string>(true, "a", "b", "c")]
            public string Value { get; set; }
        }
        private class StringNullableModel
        {
            [Validation_IncludesAttribute<string>(true, "a", "b", "c")]
            public string? Value { get; set; }
        }
        // Enum için test modelleri
        private class EnumRequiredModel
        {
            [Validation_Required]
            [Validation_IncludesAttribute<ErrorPriorityTypes>(true, ErrorPriorityTypes.normal, ErrorPriorityTypes.high)]
            public ErrorPriorityTypes Value { get; set; }
        }
        private class EnumNullableModel
        {
            [Validation_IncludesAttribute<ErrorPriorityTypes>(true, ErrorPriorityTypes.normal, ErrorPriorityTypes.high)]
            public ErrorPriorityTypes? Value { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void String_ValidValue_ShouldPass()
        {
            var model = new StringRequiredModel { Value = "b" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void String_InvalidValue_ShouldFail()
        {
            var model = new StringRequiredModel { Value = "x" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void String_RequiredEmpty_ShouldFail()
        {
            var model = new StringRequiredModel { Value = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void String_RequiredNull_ShouldFail()
        {
            var model = new StringRequiredModel { Value = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void String_NullableNull_ShouldPass()
        {
            var model = new StringNullableModel { Value = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void String_NullableEmpty_ShouldPass()
        {
            var model = new StringNullableModel { Value = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        // Enum tipli testler
        [Test]
        public void Enum_ValidValue_ShouldPass()
        {
            var model = new EnumRequiredModel { Value = ErrorPriorityTypes.high };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void Enum_InvalidValue_ShouldFail()
        {
            var model = new EnumRequiredModel { Value = ErrorPriorityTypes.catastrophicfailure };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void Enum_NullableNull_ShouldPass()
        {
            var model = new EnumNullableModel { Value = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void Enum_NullableValid_ShouldPass()
        {
            var model = new EnumNullableModel { Value = ErrorPriorityTypes.normal };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void Enum_NullableInvalid_ShouldFail()
        {
            var model = new EnumNullableModel { Value = ErrorPriorityTypes.catastrophicfailure };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
    }
}