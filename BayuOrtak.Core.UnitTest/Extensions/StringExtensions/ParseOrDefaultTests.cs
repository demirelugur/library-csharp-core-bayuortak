namespace BayuOrtak.Core.UnitTest.Extensions.StringExtensions
{
    using BayuOrtak.Core.Extensions;
    using NUnit.Framework;
    using System;
    internal enum SampleEnum
    {
        none = 0,
        first,
        second
    }
    [TestFixture]
    public class ParseOrDefaultTests
    {
        [Test]
        public void ParseOrDefault_Int_Valid()
        {
            Assert.That("123".ParseOrDefault<int>(), Is.EqualTo(123));
        }
        [Test]
        public void ParseOrDefault_Int_Invalid_ReturnsDefault()
        {
            Assert.That("abc".ParseOrDefault<int>(), Is.Zero);
        }
        [Test]
        public void ParseOrDefault_NullableInt_Valid()
        {
            var result = "456".ParseOrDefault<int?>();
            Assert.That(result, Is.EqualTo(456));
        }
        [Test]
        public void ParseOrDefault_NullableInt_Invalid_ReturnsNull()
        {
            var result = "abc".ParseOrDefault<int?>();
            Assert.That(result, Is.Null);
        }
        [Test]
        public void ParseOrDefault_Guid_Valid()
        {
            var guid = Guid.NewGuid();
            Assert.That(guid.ToString().ParseOrDefault<Guid>(), Is.EqualTo(guid));
        }
        [Test]
        public void ParseOrDefault_Guid_Invalid_ReturnsDefault()
        {
            Assert.That("not-a-guid".ParseOrDefault<Guid>(), Is.EqualTo(Guid.Empty));
        }
        [Test]
        public void ParseOrDefault_NullableGuid_Valid()
        {
            var guid = Guid.NewGuid();
            var result = guid.ToString().ParseOrDefault<Guid?>();
            Assert.That(result, Is.EqualTo(guid));
        }
        [Test]
        public void ParseOrDefault_NullableGuid_Invalid_ReturnsNull()
        {
            var result = "not-a-guid".ParseOrDefault<Guid?>();
            Assert.That(result, Is.Null);
        }
        [Test]
        public void ParseOrDefault_DateTime_Valid()
        {
            var dt = new DateTime(2024, 6, 1);
            Assert.That("2024-06-01".ParseOrDefault<DateTime>(), Is.EqualTo(dt));
        }
        [Test]
        public void ParseOrDefault_DateTime_Invalid_ReturnsDefault()
        {
            Assert.That("not-a-date".ParseOrDefault<DateTime>(), Is.Default);
        }
        [Test]
        public void ParseOrDefault_NullableDateTime_Valid()
        {
            var dt = new DateTime(2024, 6, 1);
            var result = "2024-06-01".ParseOrDefault<DateTime?>();
            Assert.That(result, Is.EqualTo(dt));
        }
        [Test]
        public void ParseOrDefault_NullableDateTime_Invalid_ReturnsNull()
        {
            var result = "not-a-date".ParseOrDefault<DateTime?>();
            Assert.That(result, Is.Null);
        }
        [Test]
        public void ParseOrDefault_Enum_Valid()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That("first".ParseOrDefault<SampleEnum>(), Is.EqualTo(SampleEnum.first));
                Assert.That("2".ParseOrDefault<SampleEnum>(), Is.EqualTo(SampleEnum.second));
            }
        }
        [Test]
        public void ParseOrDefault_Enum_Invalid_ReturnsDefault()
        {
            Assert.That("not-an-enum".ParseOrDefault<SampleEnum>(), Is.EqualTo(SampleEnum.none));
        }
        [Test]
        public void ParseOrDefault_NullableEnum_Valid()
        {
            var result = "first".ParseOrDefault<SampleEnum?>();
            Assert.That(result, Is.EqualTo(SampleEnum.first));
        }
        [Test]
        public void ParseOrDefault_NullableEnum_Invalid_ReturnsNull()
        {
            var result = "not-an-enum".ParseOrDefault<SampleEnum?>();
            Assert.That(result, Is.Null);
        }
        [Test]
        public void ParseOrDefault_String_ReturnsSame()
        {
            Assert.That("test".ParseOrDefault<string>(), Is.EqualTo("test"));
        }
        [Test]
        public void ParseOrDefault_EmptyOrNullString_ReturnsDefault()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That("".ParseOrDefault<int>(), Is.Zero);
                Assert.That("".ParseOrDefault<string>(), Is.Null);
            }
        }
    }
}