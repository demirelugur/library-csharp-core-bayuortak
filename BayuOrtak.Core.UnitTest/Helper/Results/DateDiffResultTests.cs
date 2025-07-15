namespace BayuOrtak.Core.UnitTest.Helper.Results
{
    using BayuOrtak.Core.Helper.Results;
    using NUnit.Framework;
    using System;
    [TestFixture]
    public class DateDiffResultTests
    {
        [Test]
        public void CalculateDateDifference_SameDate_ReturnsZero()
        {
            var dt = new DateTime(2024, 6, 1);
            var diff = new DateDiffResult(dt, dt);
            var result = diff.CalculateDateDifference();
            Assert.That(result, Is.EqualTo((0, 0, 0, TimeSpan.Zero)));
        }
        [Test]
        public void CalculateDateDifference_OneYearApart_ReturnsOneYear()
        {
            var diff = new DateDiffResult(new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
            var result = diff.CalculateDateDifference();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.yil, Is.EqualTo(1));
                Assert.That(result.ay, Is.Zero);
                Assert.That(result.gun, Is.Zero);
            }
        }
        [Test]
        public void CalculateDateDifference_OneMonthApart_ReturnsOneMonth()
        {
            var diff = new DateDiffResult(new DateTime(2024, 5, 1), new DateTime(2024, 6, 1));
            var result = diff.CalculateDateDifference();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.yil, Is.Zero);
                Assert.That(result.ay, Is.EqualTo(1));
                Assert.That(result.gun, Is.Zero);
            }
        }
        [Test]
        public void CalculateDateDifference_OneDayApart_ReturnsOneDay()
        {
            var diff = new DateDiffResult(new DateTime(2024, 6, 1), new DateTime(2024, 6, 2));
            var result = diff.CalculateDateDifference();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.yil, Is.Zero);
                Assert.That(result.ay, Is.Zero);
                Assert.That(result.gun, Is.EqualTo(1));
            }
        }
        [Test]
        public void CalculateDateDifference_NegativeOrReverseDates_ReturnsCorrectDifference()
        {
            var diff = new DateDiffResult(new DateTime(2024, 6, 2), new DateTime(2024, 6, 1));
            var result = diff.CalculateDateDifference();
            Assert.That(Math.Abs(result.gun), Is.EqualTo(0));
        }
        [Test]
        public void CalculateDateDifference_WithTimeSpan_ReturnsTimeDifference()
        {
            var diff = new DateDiffResult(new DateTime(2024, 6, 1, 8, 0, 0), new DateTime(2024, 6, 1, 10, 30, 0));
            var result = diff.CalculateDateDifference();
            Assert.That(result.ts, Is.EqualTo(new TimeSpan(2, 30, 0)));
        }
        [Test]
        public void CalculateDateDifference_StringAndNullInputs_ParsesCorrectly()
        {
            var diff = new DateDiffResult("2024-06-01", null);
            var today = DateTime.Today;
            var result = diff.CalculateDateDifference();
            Assert.That(result.yil, Is.EqualTo(today.Year - 2024));
        }
        [Test]
        public void FormatDateDifference_FormatsCorrectly()
        {
            var diff = new DateDiffResult(new DateTime(2020, 1, 1), new DateTime(2021, 2, 3));
            var result = diff.FormatDateDifference();
            Assert.That(result, Is.EqualTo("1 yıl, 1 ay, 2 gün"));
        }
        [Test]
        public void FormatDateDifference_NoDifference_ReturnsEmptyString()
        {
            var dt = new DateTime(2024, 6, 1);
            var diff = new DateDiffResult(dt, dt);
            var result = diff.FormatDateDifference();
            Assert.That(result, Is.EqualTo(""));
        }
    }
}