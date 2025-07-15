namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_ArrayMinLengthAttributeTests
    {
        private class RequiredTestModel
        {
            [Validation_Required]
            [Validation_ArrayMinLength(2)]
            public string[] items { get; set; }
        }
        private class NullableTestModel
        {
            [Validation_ArrayMinLength(2)]
            public string[] items { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void ArrayMinLength_ValidCollectionWithEnoughItems_ShouldPass()
        {
            var model = new RequiredTestModel { items = new string[] { "item1", "item2", "item3" } };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void ArrayMinLength_CollectionWithFewerItems_ShouldFail()
        {
            var model = new RequiredTestModel { items = new string[] { "item1" } };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void ArrayMinLength_RequiredEmptyCollection_ShouldFail()
        {
            var model = new RequiredTestModel { items = Array.Empty<string>() };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void ArrayMinLength_RequiredNullCollection_ShouldFail()
        {
            var model = new RequiredTestModel { items = null };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void ArrayMinLength_NullableEmptyCollection_ShouldFail()
        {
            var model = new NullableTestModel { items = Array.Empty<string>() };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void ArrayMinLength_NullableNullCollection_ShouldPass()
        {
            var model = new NullableTestModel { items = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}