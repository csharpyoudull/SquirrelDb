using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDb.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var exit = false;           
            DisplayMenu();
            
            while (!exit)
            {
                var option = System.Console.ReadLine();
                if (string.IsNullOrEmpty(option))
                    continue;
                
                if (option.Equals("exit",StringComparison.OrdinalIgnoreCase))
                {
                    exit = true;
                    continue;
                }

                if (option.StartsWith("add",StringComparison.OrdinalIgnoreCase))
                    Add(option);

                if (option.StartsWith("get", StringComparison.OrdinalIgnoreCase))
                    Get(option);

                if (option.StartsWith("cbucket", StringComparison.OrdinalIgnoreCase))
                    CreateBucket(option);

                if (option.StartsWith("load", StringComparison.OrdinalIgnoreCase))
                    CreateBucket(option);

            }
        }

        static void LoadFile(string commandLine)
        {
            commandLine = commandLine.Remove(0, 5);

            var client = new Client();
            var input = File.ReadAllLines(commandLine).Select(ln => ln.Split('|'));
            var watch = Stopwatch.StartNew();
            Parallel.ForEach(input, item => client.AddDocument(item[0], item[1], item[2]));
            watch.Stop();

            System.Console.WriteLine("{0} records inserted.",input.Count());
            System.Console.WriteLine("Operation took {0} milliseconds", watch.ElapsedMilliseconds);
        }

        static void CreateBucket(string commandLine)
        {
            commandLine = commandLine.Remove(0, 8);
            var kv = commandLine.Split('|');
            var client = new Client();
            var watch = Stopwatch.StartNew();
            var result = client.CreateBucket(kv[0],int.Parse(kv[1]),int.Parse(kv[2]));
            watch.Stop();

            System.Console.WriteLine("Create bucket {0} operation took {1} milliseconds", result ? "successful" : "failed", watch.ElapsedMilliseconds);
        }

        static void Add(string commandLine)
        {
            commandLine = commandLine.Remove(0, 4);
            var kv = commandLine.Split('|');
            var client = new Client();
            var watch = Stopwatch.StartNew();
            var result = client.AddDocument(kv[0], kv[1], kv[2]);
            watch.Stop();

            System.Console.WriteLine("Write {0} operation took {1} milliseconds",result ? "successful" : "failed",watch.ElapsedMilliseconds);
        }

        static void Get(string commandLine)
        {
            commandLine = commandLine.Remove(0, 4);
            var kv = commandLine.Split('|');
            var client = new Client();
            var watch = Stopwatch.StartNew();
            var result = client.GetDocument(kv[0], kv[1]);
            watch.Stop();
            System.Console.WriteLine(string.IsNullOrEmpty(result) ? "Record not found." : result);
            System.Console.WriteLine("Operation took {0} milliseconds", watch.ElapsedMilliseconds);
        }

        static void DisplayMenu()
        {
            System.Console.WriteLine("Options");
            System.Console.WriteLine("--------------------------");
            System.Console.WriteLine("Add: add bucket|key|value");
            System.Console.WriteLine("Get: get bucket|key");
            System.Console.WriteLine("CBucket: bucketName|maxRecordSize|maxRecordsPerBin");           
            System.Console.WriteLine("--------------------------");
            System.Console.WriteLine("Load: file");
            System.Console.WriteLine("-Files must be in pipe delimited bucket|key|value format.");
            System.Console.WriteLine("--------------------------");
            System.Console.WriteLine("Menu: to display menu.");
            System.Console.WriteLine("Exit: to exit application.");
        }
    }
}
