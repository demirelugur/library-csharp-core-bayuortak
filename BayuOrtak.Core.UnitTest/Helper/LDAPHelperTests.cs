using BayuOrtak.Core.Enums;
namespace BayuOrtak.Core.UnitTest.Helper
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    [TestFixture]
    public class LDAPHelperTests
    {
        private LDAPHelper _ldapHelper;
        [SetUp]
        public void Setup()
        {
            _ldapHelper = new LDAPHelper("baydc", "uzem");
        }
        [Test]
        public void Check()
        {
            var _l = _ldapHelper.Check(LDAPTip.personelkurum, "username", "password", true, "tr");
            Assert.That(_l.statuswarning, Is.True);
            if (_l.statuswarning) { foreach (var item in _l.ex.AllExceptionMessage()) { Assert.Pass(item); } }
        }
    }
} 