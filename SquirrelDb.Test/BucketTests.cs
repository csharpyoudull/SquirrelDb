using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SquirrelDb.Test
{
    [TestClass]
    public class BucketTests
    {
        [TestMethod]
        public void CreateBucket()
        {
            var bucket = DataBucket.CreateNewBucket("TestBucket", "../../../Db/", 1024, 50);
            Assert.IsTrue(Directory.Exists(Path.Combine("../../../Db/", "TestBucket")));
        }

        [TestMethod]
        public void LoadBucket()
        {
            var bucket = DataBucket.LoadDataBucket("TestBucket", "../../../Db/");
            Assert.IsTrue(bucket != null);
        }
    }
}
