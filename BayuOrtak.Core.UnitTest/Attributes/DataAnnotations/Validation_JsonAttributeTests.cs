namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_JsonAttributeTests
    {
        private class JObjectRequiredModel
        {
            [Validation_Required]
            [Validation_Json(JTokenType.Object)]
            public string Json { get; set; }
        }
        private class JObjectNullableModel
        {
            [Validation_Json(JTokenType.Object)]
            public string? Json { get; set; }
        }
        private class JArrayRequiredModel
        {
            [Validation_Required]
            [Validation_Json(JTokenType.Array)]
            public string Json { get; set; }
        }
        private class JArrayNullableModel
        {
            [Validation_Json(JTokenType.Array)]
            public string? Json { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        // JObject
        [Test]
        public void JObject_Valid_ShouldPass()
        {
            var model = new JObjectRequiredModel { Json = "{\"a\":1}" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void JObject_Invalid_ShouldFail()
        {
            var model = new JObjectRequiredModel { Json = "[1,2,3]" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void JObject_RequiredEmpty_ShouldFail()
        {
            var model = new JObjectRequiredModel { Json = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void JObject_RequiredNull_ShouldFail()
        {
            var model = new JObjectRequiredModel { Json = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void JObject_NullableNull_ShouldPass()
        {
            var model = new JObjectNullableModel { Json = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void JObject_NullableEmpty_ShouldPass()
        {
            var model = new JObjectNullableModel { Json = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        // JArray
        [Test]
        public void JArray_Valid_ShouldPass()
        {
            var model = new JArrayRequiredModel { Json = "[1,2,3]" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void JArray_Invalid_ShouldFail()
        {
            var model = new JArrayRequiredModel { Json = "{\"a\":1}" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void JArray_RequiredEmpty_ShouldFail()
        {
            var model = new JArrayRequiredModel { Json = "" };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void JArray_RequiredNull_ShouldFail()
        {
            var model = new JArrayRequiredModel { Json = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void JArray_NullableNull_ShouldPass()
        {
            var model = new JArrayNullableModel { Json = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void JArray_NullableEmpty_ShouldPass()
        {
            var model = new JArrayNullableModel { Json = "" };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}