using NUnit.Framework;

namespace Embroidery.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TokenizeName()
        {
            var result = Embroidery.Client.IO.Execution.TokenizeName("HAB-HeartStarsZZ 5x7.pes");


            foreach (var token in result)
                System.Diagnostics.Debug.WriteLine(token);

            Assert.AreEqual("HAB", result[0]);
            Assert.AreEqual("Heart", result[1]);
            Assert.AreEqual("Stars", result[2]);
            Assert.AreEqual("ZZ", result[3]);
            Assert.AreEqual("pes", result[4]);
        }
    }
}