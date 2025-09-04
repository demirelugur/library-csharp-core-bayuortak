namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_ISBNAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_ISBN]
            public string ISBN { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_ISBN]
            public string? ISBN { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void ISBN_ValidISBN10_ShouldPass()
        {
            var model = new RequiredTestModel { ISBN = "0-306-40615-2" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void ISBN_ValidISBN13_ShouldPass()
        {
            var model = new RequiredTestModel { ISBN = "978-3-16-148410-0" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void ISBN_InvalidISBN_ShouldFail()
        {
            var model = new RequiredTestModel { ISBN = "1234567890" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void ISBN_RequiredEmpty_ShouldFail()
        {
            var model = new RequiredTestModel { ISBN = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void ISBN_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModel { ISBN = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void ISBN_NullableNull_ShouldPass()
        {
            var model = new NullableTestModel { ISBN = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void ISBN_NullableEmpty_ShouldPass()
        {
            var model = new NullableTestModel { ISBN = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void ISBN_NullableInvalid_ShouldFail()
        {
            var model = new NullableTestModel { ISBN = "invalid-isbn" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
    }
}