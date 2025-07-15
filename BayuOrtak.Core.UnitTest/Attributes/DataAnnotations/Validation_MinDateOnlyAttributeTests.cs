namespace BayuOrtak.Core.UnitTest.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using System.ComponentModel.DataAnnotations;
    [TestFixture]
    public class Validation_MinDateOnlyAttributeTests
    {
        private static readonly DateOnly BayburtUniFoundation = new DateOnly(2008, 6, 1);
        private static readonly DateOnly CustomMinDate = new DateOnly(2020, 1, 1);
        private class RequiredTestModelDefault
        {
            [Validation_Required]
            [Validation_MinDateOnly]
            public DateOnly Date { get; set; }
        }
        private class NullableTestModelDefault
        {
            [Validation_MinDateOnly]
            public DateOnly? Date { get; set; }
        }
        private class RequiredTestModelCustom
        {
            [Validation_Required]
            [Validation_MinDateOnly("2020-01-01")]
            public DateOnly Date { get; set; }
        }
        private class NullableTestModelCustom
        {
            [Validation_MinDateOnly("2020-01-01")]
            public DateOnly? Date { get; set; }
        }
        private class RequiredTestModelDateTime
        {
            [Validation_Required]
            [Validation_MinDateOnly("2020-01-01")]
            public DateTime Date { get; set; }
        }
        private class NullableTestModelDateTime
        {
            [Validation_MinDateOnly("2020-01-01")]
            public DateTime? Date { get; set; }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
        [Test]
        public void MinDateOnly_Default_Valid_ShouldPass()
        {
            var model = new RequiredTestModelDefault { Date = BayburtUniFoundation.AddDays(1) };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Default_Equal_ShouldPass()
        {
            var model = new RequiredTestModelDefault { Date = BayburtUniFoundation };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Default_Before_ShouldFail()
        {
            var model = new RequiredTestModelDefault { Date = BayburtUniFoundation.AddDays(-1) };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MinDateOnly_Default_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModelDefault { Date = default };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MinDateOnly_Default_NullableNull_ShouldPass()
        {
            var model = new NullableTestModelDefault { Date = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Custom_Valid_ShouldPass()
        {
            var model = new RequiredTestModelCustom { Date = CustomMinDate.AddDays(10) };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Custom_Equal_ShouldPass()
        {
            var model = new RequiredTestModelCustom { Date = CustomMinDate };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Custom_Before_ShouldFail()
        {
            var model = new RequiredTestModelCustom { Date = CustomMinDate.AddDays(-1) };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MinDateOnly_Custom_RequiredNull_ShouldFail()
        {
            var model = new RequiredTestModelCustom { Date = default };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MinDateOnly_Custom_NullableNull_ShouldPass()
        {
            var model = new NullableTestModelCustom { Date = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Custom_DateTime_Valid_ShouldPass()
        {
            var model = new RequiredTestModelDateTime { Date = new DateTime(2020, 1, 2) };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Custom_DateTime_Equal_ShouldPass()
        {
            var model = new RequiredTestModelDateTime { Date = new DateTime(2020, 1, 1) };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
        [Test]
        public void MinDateOnly_Custom_DateTime_Before_ShouldFail()
        {
            var model = new RequiredTestModelDateTime { Date = new DateTime(2019, 12, 31) };
            var results = ValidateModel(model);
            Assert.That(results, Has.Count.GreaterThan(0));
            foreach (var item in results) { Assert.Pass(item.ErrorMessage); }
        }
        [Test]
        public void MinDateOnly_Custom_DateTime_NullableNull_ShouldPass()
        {
            var model = new NullableTestModelDateTime { Date = null };
            var results = ValidateModel(model);
            Assert.That(results, Is.Empty);
        }
    }
}