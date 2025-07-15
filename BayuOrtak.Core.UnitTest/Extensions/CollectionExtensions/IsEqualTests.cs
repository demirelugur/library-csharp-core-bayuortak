namespace BayuOrtak.Core.UnitTest.Extensions.CollectionExtensions
{
    using BayuOrtak.Core.Extensions;
    using NUnit.Framework;
    using System.Collections.Generic;
    [TestFixture]
    public class IsEqualTests
    {
        [Test]
        public void IsEqual_SameCollections_ReturnsTrue()
        {
            var a = new List<int> { 1, 2, 3 };
            var b = new List<int> { 1, 2, 3 };
            Assert.That(a.IsEqual(b), Is.True);
        }
        [Test]
        public void IsEqual_DifferentCollections_ReturnsFalse()
        {
            var a = new List<int> { 1, 2, 3 };
            var b = new List<int> { 1, 2, 4 };
            Assert.That(a.IsEqual(b), Is.False);
        }

        [Test]
        public void IsEqual_SameElementsDifferentOrder_ReturnsTrue()
        {
            var a = new List<int> { 1, 2, 3 };
            var b = new List<int> { 3, 2, 1 };
            Assert.That(a.IsEqual(b), Is.True);
        }
        [Test]
        public void IsEqual_DifferentCounts_ReturnsFalse()
        {
            var a = new List<int> { 1, 2, 3 };
            var b = new List<int> { 1, 2 };
            Assert.That(a.IsEqual(b), Is.False);
        }
        [Test]
        public void IsEqual_BothNull_ReturnsTrue()
        {
            List<int> a = null;
            List<int> b = null;
            Assert.That(a.IsEqual(b), Is.True);
        }
        [Test]
        public void IsEqual_OneNullOneEmpty_ReturnsFalse()
        {
            List<int> a = null;
            var b = new List<int>();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(a.IsEqual(b), Is.False);
                Assert.That(b.IsEqual(a), Is.False);
            }
        }
        [Test]
        public void IsEqual_BothEmpty_ReturnsTrue()
        {
            var a = new List<int>();
            var b = new List<int>();
            Assert.That(a.IsEqual(b), Is.True);
        }
    }
}