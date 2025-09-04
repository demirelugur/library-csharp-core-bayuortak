namespace BayuOrtak.Core.UnitTest.Helper
{
    using BayuOrtak.Core.Enums;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    [TestFixture]
    public class LDAPHelperTests
    {
        //private long _ogrtckn;
        //private string _kimlikserino;
        //private DateOnly _dogumtarih;
        //private Nvi_KimlikTypes _kimliktipi;
        //private string[] _ogrnos;
        private LDAPHelper _ldapHelper;
        [SetUp]
        public void Setup()
        {
            //_ogrtckn = 19681556898; // Pýnar BALUKEN
            //_kimlikserino = "A30P93844";
            //_kimliktipi = Nvi_KimlikTypes.yeni;
            //_dogumtarih = new DateOnly(1986, 5, 26);
            //_ogrnos = "091852047,130102006,161205003,182003008,191713003,232206002".Split(',');
            _ldapHelper = new LDAPHelper("", "");
        }
        [Test]
        public void Check()
        {
            //foreach (var item in _ogrnos) { var _c = _ldapHelper.Search(LDAPTip.stu, item, false, "tr"); }
            //var _l = _ldapHelper.Check(LDAPTip.personelkurum, "username", "password", true, "tr");
            //Assert.That(_l.statuswarning, Is.True);
            //if (_l.statuswarning) { foreach (var item in _l.ex.AllExceptionMessage()) { Assert.Pass(item); } }
            Assert.Pass();
        }
    }
}