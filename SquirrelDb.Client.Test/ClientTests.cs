using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SquirrelDb.Client.Test
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void CreateBucketTest()
        {
            var client = new Client();
            var result = client.CreateBucket("TestBucket", 1024, 250000);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WriteEntryTest()
        {
            var client = new Client();
            var result =client.AddDocument("TestBucket", "TestKey", "This is some sample text for a test.");
        
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetEntryTest()
        {
            var client = new Client();
            var result = client.GetDocument("TestBucket", "TestKey");

            Assert.IsTrue(result.Equals("This is some sample text for a test."));
        }
    }
}
