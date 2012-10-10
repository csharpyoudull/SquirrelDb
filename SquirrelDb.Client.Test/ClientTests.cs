using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquirrelDb.Client.Requests;

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
            var result =client.StoreDocument(new  WriteDocRequest(){BucketName ="TestBucket", Key ="TestKey", Value="This is some sample text for a test."});

            Assert.IsTrue(result == HttpStatusCode.OK);
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
