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
            var bucket = DataBucket.CreateNewBucket("TestBucket", "../../../Db/", 1024, 250000);
            Assert.IsTrue(Directory.Exists(Path.Combine("../../../Db/", "TestBucket")));
        }

        [TestMethod]
        public void LoadBucket()
        {
            var bucket = DataBucket.LoadDataBucket("TestBucket", "../../../Db/");
            Assert.IsTrue(bucket != null);
        }

        [TestMethod]
        public void AddEntryPass()
        {
            try
            {
                var bucket = DataBucket.LoadDataBucket("TestBucket", "../../../Db/");
                bucket.Add("TestKey1", "This is test data for the document database.");

                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            
        }

        [TestMethod]
        public void GetEntryPass()
        {
            try
            {
                var bucket = DataBucket.LoadDataBucket("TestBucket", "../../../Db/");
                var result = bucket.Get("TestKey1");
                Assert.IsTrue(result.Equals("This is test data for the document database."));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void AddEntryFail()
        {
            try
            {
                var bucket = DataBucket.LoadDataBucket("TestBucket", "../../../Db/");
                bucket.Add("TestKey1", "This is test data for the document database.");
                bucket.Add("TestKey1", "This is test data for the document database.");

                Assert.Fail("This should not be possible");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            }

        }


    }
}
