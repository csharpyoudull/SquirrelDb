using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Linq;

namespace SquirrelDb.Test
{
    [TestClass]
    public class MappedFileTests
    {
        [TestMethod]
        public void CreateMappedFile()
        {
            var map = MappedFile.CreateNewMap("map1.dat", "../../../Db/", 1024, 20);
            Assert.IsTrue(File.Exists("../../../Db/map1.dat") && File.Exists("../../../Db/map1.dat.config"));
        }

        [TestMethod]
        public void LoadMappedfile()
        {
            try
            {
                var map = MappedFile.LoadMap("map1.dat", "../../../Db/");
                Assert.IsTrue(map != null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void WriteMapTest()
        {
            try
            {
                var map = MappedFile.LoadMap("map1.dat", "../../../Db/");
                var doc = new TestDocument
                              {
                                  FirstName = "Steve",
                                  LastName = "Ruben",
                                  DateOfBirth = new DateTime(1891, 10, 11),
                                  Height = 5.6,
                                  Weight = 200.00
                              };

                var docJson = JsonConvert.SerializeObject(doc);
                var index = map.Write(docJson);

                Assert.IsTrue(index != -1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void WriteManyMapTest()
        {
            try
            {
                var map = MappedFile.LoadMap("map1.dat", "../../../Db/");
                var doc = new TestDocument
                {
                    FirstName = "Steve",
                    LastName = "Ruben",
                    DateOfBirth = new DateTime(1891, 10, 11),
                    Height = 5.6,
                    Weight = 200.00
                };

                var docJson = JsonConvert.SerializeObject(doc);
                var docs = new List<string>
                               {
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson,
                                   docJson
                               };

                var indicies = docs.AsParallel().Select(map.Write).ToList();
                
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ReadMapTest()
        {
            try
            {
                var map = MappedFile.LoadMap("map1.dat", "../../../Db/");
                var doc = new TestDocument
                {
                    FirstName = "Steve",
                    LastName = "Ruben",
                    DateOfBirth = new DateTime(1891, 10, 11),
                    Height = 5.6,
                    Weight = 200.00
                };

                var dbDoc = JsonConvert.DeserializeObject<TestDocument>(map.Read(0));
                Assert.IsTrue(dbDoc.FirstName.Equals(doc.FirstName));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ReadExactLengthMapTest()
        {
            try
            {
                var map = MappedFile.LoadMap("map1.dat", "../../../Db/");
                var doc = new TestDocument
                {
                    FirstName = "Steve",
                    LastName = "Ruben",
                    DateOfBirth = new DateTime(1891, 10, 11),
                    Height = 5.6,
                    Weight = 200.00
                };

                var docJson = JsonConvert.SerializeObject(doc);
                var dbDoc = JsonConvert.DeserializeObject<TestDocument>(map.Read(0,docJson.Length));
                Assert.IsTrue(dbDoc.FirstName.Equals(doc.FirstName));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void DeleteTest()
        {
            try
            {
                var map = MappedFile.LoadMap("map1.dat", "../../../Db/");
                map.Delete(0);
                var dbDoc = JsonConvert.DeserializeObject<TestDocument>(map.Read(0));
                
                Assert.IsNull(dbDoc);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
