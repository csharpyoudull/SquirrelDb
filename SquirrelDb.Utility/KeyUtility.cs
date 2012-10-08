using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDb.Utility
{
    public class KeyUtility
    {
        public static byte[] GetKeyHash(string key)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(Encoding.ASCII.GetBytes(key));
        }
    }
}
