using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDb.Utility
{
    public class StringUtilities
    {
        public static string GenerateRandomString(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random(DateTime.Now.Millisecond);
            var output = string.Empty;
            for(var i = 0; i < length; i ++)
            {
                output += characters[random.Next(characters.Length - 1)];
            }

            return output;
        }
    }
}
