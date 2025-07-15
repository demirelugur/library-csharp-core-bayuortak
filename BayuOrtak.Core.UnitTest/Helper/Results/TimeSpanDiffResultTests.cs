namespace BayuOrtak.Core.UnitTest.Helper.Results
{
    using BayuOrtak.Core.Helper.Results;
    using NUnit.Framework;
    using System;
    [TestFixture]
    public class TimeSpanDiffResultTests
    {
        [Test]
        public void DecomposeTimeSpan_Zero_ReturnsAllZero()
        {
            var ts = new TimeSpanDiffResult(TimeSpan.Zero);
            var d = ts.DecomposeTimeSpan;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(d.totalhours, Is.Zero);
                Assert.That(d.minutes, Is.Zero);
                Assert.That(d.seconds, Is.Zero);
                Assert.That(d.milliseconds, Is.Zero);
            }
        }
        [Test]
        public void DecomposeTimeSpan_OnlySeconds()
        {
            var ts = new TimeSpanDiffResult(TimeSpan.FromSeconds(45));
            var d = ts.DecomposeTimeSpan;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(d.totalhours, Is.Zero);
                Assert.That(d.minutes, Is.Zero);
                Assert.That(d.seconds, Is.EqualTo(45));
            }
        }
        [Test]
        public void DecomposeTimeSpan_MinutesAndSeconds()
        {
            var ts = new TimeSpanDiffResult(new TimeSpan(0, 2, 30));
            var d = ts.DecomposeTimeSpan;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(d.totalhours, Is.Zero);
                Assert.That(d.minutes, Is.EqualTo(2));
                Assert.That(d.seconds, Is.EqualTo(30));
            }
        }
        [Test]
        public void DecomposeTimeSpan_HoursMinutesSeconds()
        {
            var ts = new TimeSpanDiffResult(new TimeSpan(1, 15, 40));
            var d = ts.DecomposeTimeSpan;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(d.totalhours, Is.EqualTo(1));
                Assert.That(d.minutes, Is.EqualTo(15));
                Assert.That(d.seconds, Is.EqualTo(40));
            }
        }
        [Test]
        public void DecomposeTimeSpan_WithMilliseconds()
        {
            var ts = new TimeSpanDiffResult(new TimeSpan(0, 0, 5, 12, 123));
            var d = ts.DecomposeTimeSpan;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(d.seconds, Is.EqualTo(12));
                Assert.That(d.milliseconds, Is.EqualTo(123));
            }
        }
        [Test]
        public void DecomposeTimeSpan_NegativeValues()
        {
            var ts = new TimeSpanDiffResult(TimeSpan.FromSeconds(-30));
            var d = ts.DecomposeTimeSpan;
            Assert.That(d.seconds, Is.EqualTo(-30));
        }
        [Test]
        public void Constructor_TimeOnly_ConvertsCorrectly()
        {
            var to = new TimeOnly(2, 30, 15);
            var ts = new TimeSpanDiffResult(to);
            var d = ts.DecomposeTimeSpan;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(d.totalhours, Is.EqualTo(2));
                Assert.That(d.minutes, Is.EqualTo(30));
                Assert.That(d.seconds, Is.EqualTo(15));
            }
        }
        [Test]
        public void FormatTimeSpan_Zero_ReturnsZeroSeconds()
        {
            var ts = new TimeSpanDiffResult(TimeSpan.Zero);
            var result = ts.FormatTimeSpan();
            Assert.That(result, Is.EqualTo("0 sn."));
        }
        [Test]
        public void FormatTimeSpan_HoursMinutesSeconds()
        {
            var ts = new TimeSpanDiffResult(new TimeSpan(1, 2, 3));
            var result = ts.FormatTimeSpan();
            Assert.That(result, Is.EqualTo("1 saat 2 dk. 3 sn."));
        }
        [Test]
        public void FormatTimeSpan_WithMilliseconds()
        {
            var ts = new TimeSpanDiffResult(new TimeSpan(0, 0, 0, 5, 12));
            var result = ts.FormatTimeSpan();
            Assert.That(result, Is.EqualTo("5,012 sn."));
        }
        [Test]
        public void FormatTimeSpan_OnlyMilliseconds()
        {
            var ts = new TimeSpanDiffResult(TimeSpan.FromMilliseconds(250));
            var result = ts.FormatTimeSpan();
            Assert.That(result, Is.EqualTo("0,250 sn."));
        }
    }
}