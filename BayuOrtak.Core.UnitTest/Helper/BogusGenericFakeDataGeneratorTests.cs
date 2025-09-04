namespace BayuOrtak.Core.UnitTest.Helper
{
    using BayuOrtak.Core.Helper;
    [TestFixture]
    public class BogusGenericFakeDataGeneratorTests
    {
        public class Search_response
        {
            public string ogrencino { get; set; }
            public string dbinfo { get; set; }
            public DateOnly? mezundate { get; set; }
            public bool status { get; set; }
            public bool isaccountdisable { get; set; }
            public bool ispasswordexpired { get; set; }
            public string eposta => $"o{ogrencino}@stu.bayburt.edu.tr";
            public Search_response(string ogrencino, string dbinfo, DateOnly? mezundate, bool status, bool isaccountdisable, bool ispasswordexpired)
            {
                this.ogrencino = ogrencino;
                this.dbinfo = dbinfo;
                this.mezundate = mezundate;
                this.status = status;
                this.isaccountdisable = isaccountdisable;
                this.ispasswordexpired = ispasswordexpired;
            }
        }
        [Test]
        public void Check()
        {
            var c = new BogusGenericFakeDataGenerator().GenerateArray<Search_response>(4);
            Assert.Pass();
        }
    }
}