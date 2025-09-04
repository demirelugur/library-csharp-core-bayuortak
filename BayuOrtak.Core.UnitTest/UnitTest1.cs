namespace BayuOrtak.Core.UnitTest
{
    using System.Threading.Tasks;
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
        [Test]
        public async Task Test2()
        {
            await Task.CompletedTask;
            Assert.Pass();
        }
        [TearDown]
        public void Cleanup()
        {

        }
    }
}