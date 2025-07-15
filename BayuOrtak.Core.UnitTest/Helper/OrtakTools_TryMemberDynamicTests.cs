namespace BayuOrtak.Core.UnitTest.Helper
{
    using NUnit.Framework;
    using System.Dynamic;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [TestFixture]
    public class OrtakTools_TryMemberDynamicTests
    {
        [Test]
        public void TryMemberDynamic_ValidKeyAndType_ReturnsTrueAndValue()
        {
            dynamic expando = new ExpandoObject();
            expando.Name = "TestName";
            var result = _try.TryMemberDynamic(expando, "Name", out string value);
            Assert.That(result, Is.True);
            Assert.That(value, Is.EqualTo("TestName"));
        }
        [Test]
        public void TryMemberDynamic_ValidKeyButWrongType_ReturnsFalseAndDefault()
        {
            dynamic expando = new ExpandoObject();
            expando.Age = 42;
            var result = _try.TryMemberDynamic(expando, "Age", out string _);
            Assert.That(result, Is.False);
        }
        [Test]
        public void TryMemberDynamic_InvalidKey_ReturnsFalseAndDefault()
        {
            dynamic expando = new ExpandoObject();
            expando.Exists = 123;
            var result = _try.TryMemberDynamic(expando, "NotExists", out int _);
            Assert.That(result, Is.False);
        }
        [Test]
        public void TryMemberDynamic_NullKey_ReturnsFalseAndDefault()
        {
            dynamic expando = new ExpandoObject();
            expando.Value = 10;
            var result = _try.TryMemberDynamic(expando, null, out int _);
            Assert.That(result, Is.False);
        }
        [Test]
        public void TryMemberDynamic_EmptyKey_ReturnsFalseAndDefault()
        {
            dynamic expando = new ExpandoObject();
            expando.Value = 10;
            var result = _try.TryMemberDynamic(expando, "", out int _);
            Assert.That(result, Is.False);
        }
        [Test]
        public void TryMemberDynamic_NullExpando_ReturnsFalseAndDefault()
        {
            ExpandoObject expando = null;
            var result = _try.TryMemberDynamic(expando, "AnyKey", out int _);
            Assert.That(result, Is.False);
        }
    }
}