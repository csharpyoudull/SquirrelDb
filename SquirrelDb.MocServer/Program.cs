using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDb.MocServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server.");
            ServerNode.ActivateDatabase();
            Console.WriteLine("Server online.");
            Console.ReadLine();
        }
    }
}
