using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SquirrelDb.Utility.Test
{
    [TestClass]
    public class KeyUtilityTests
    {
        [TestMethod]
        public void ComputeHashTest()
        {
            const string key = "SteveRuben-SuperDuper-Key";
            var keyHash = KeyUtility.GetKeyHash(key);

            Assert.IsTrue(keyHash.Length > 0);
        }
    }
}
