using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SquirrelDb.Client.Requests;

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

                if (option.Equals("menu", StringComparison.OrdinalIgnoreCase))
                {
                    System.Console.Clear();
                    DisplayMenu();
                }

                if (option.StartsWith("add",StringComparison.OrdinalIgnoreCase))
                    Add(option);

                if (option.StartsWith("get", StringComparison.OrdinalIgnoreCase))
                    Get(option);
                
                if (option.StartsWith("del", StringComparison.OrdinalIgnoreCase))
                    Del(option);

                if (option.StartsWith("cbucket", StringComparison.OrdinalIgnoreCase))
                    CreateBucket(option);

                if (option.StartsWith("file add", StringComparison.OrdinalIgnoreCase))
                    LoadFile(option);

                if (option.StartsWith("file get", StringComparison.OrdinalIgnoreCase))
                    GetFile(option);

                if (option.StartsWith("bulk add", StringComparison.OrdinalIgnoreCase))
                    BulkAdd(option);

                if (option.StartsWith("bulk get", StringComparison.OrdinalIgnoreCase))
                    BulkGet(option);

            }
        }

        static void LoadFile(string commandLine)
        {
            commandLine = commandLine.Remove(0, 9);

            var client = new Client();
            var input = File.ReadAllLines(commandLine).Select(ln => ln.Split('|'));
            var request = input.Select(i => new WriteDocRequest {BucketName = i[0], Key = i[1], Value = i[2]}).ToList();
            var watch = Stopwatch.StartNew();
            foreach (var r in request)
                client.StoreDocument(r);
            watch.Stop();

            System.Console.WriteLine("{0} records inserted.",input.Count());
            System.Console.WriteLine("Operation took {0} milliseconds", watch.ElapsedMilliseconds);
        }

        static void BulkGet(string commandLine)
        {
            commandLine = commandLine.Remove(0, 9);
            var client = new Client();

            var kv = commandLine.Split('|');
            var bucket = kv[0];
            var keyHead = kv[1];
            var from = int.Parse(kv[2]);
            var to = int.Parse(kv[3]);

            var keys = new List<string>();
            for (var i = from; i < to; i ++)
                keys.Add(keyHead + i);

            var reuqest = new GetMultipleRequest {BucketName = bucket, Keys = keys};

            var watch = Stopwatch.StartNew(); 
            var result = client.GetDocuemnts(reuqest);
            watch.Stop();
            System.Console.WriteLine("{0} results returned.", result.Count);
            System.Console.WriteLine("{0} documents found, {1} documents not found", result.Count(data => !string.IsNullOrEmpty(data.Value)), result.Count(data => string.IsNullOrEmpty(data.Value)));
            System.Console.WriteLine("Operation took {0} milliseconds", watch.ElapsedMilliseconds);
        }

        static void BulkAdd(string commandLine)
        {
            commandLine = commandLine.Remove(0, 9);
            var client = new Client();
            
            var kv = commandLine.Split('|');
            var bucket = kv[0];
            var keyHead = kv[1];
            var from = int.Parse(kv[2]);
            var to = int.Parse(kv[3]);
            var request = new List<WriteDocRequest>(to);
            
            var watch = Stopwatch.StartNew();
            Parallel.For(from, to, i =>
                                       {
                                           client.StoreDocument(new WriteDocRequest { BucketName = bucket, Key = keyHead + i, Value = i.ToString() });                
                                       });
                                
            watch.Stop();
            System.Console.WriteLine("{0} records inserted.", to - from);
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

            System.Console.WriteLine("Create bucket {0} operation took {1} milliseconds", result == HttpStatusCode.OK ? "successful" : "failed", watch.ElapsedMilliseconds);
        }

        static void Add(string commandLine)
        {
            commandLine = commandLine.Remove(0, 4);
            var kv = commandLine.Split('|');
            var client = new Client();
            var watch = Stopwatch.StartNew();
            var result = client.StoreDocument(new WriteDocRequest{BucketName =kv[0],Key = kv[1], Value =kv[2]});
            watch.Stop();

            System.Console.WriteLine("Write {0} operation took {1} milliseconds",result == HttpStatusCode.OK ? "successful" : "failed",watch.ElapsedMilliseconds);
        }

        static void Add(string bucket, string key, string value)
        {
            var client = new Client();
            var result = client.StoreDocument(new WriteDocRequest { BucketName = bucket, Key = key, Value = value });
        }


        static void Del(string commandLine)
        {
            commandLine = commandLine.Remove(0, 4);
            var kv = commandLine.Split('|');
            var client = new Client();
            var watch = Stopwatch.StartNew();
            var result = client.DeleteDocument( new DeleteRequest { BucketName = kv[0], Key = kv[1] });
            watch.Stop();

            System.Console.WriteLine("Write {0} operation took {1} milliseconds", result == HttpStatusCode.OK ? "successful" : "failed", watch.ElapsedMilliseconds);
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

        static void GetFile(string commandLine)
        {
            commandLine = commandLine.Remove(0, 9);
            var kv = File.ReadAllLines(commandLine).Select(ln => ln.Split('|')).ToList();
            var lines = kv.Select(k => k[1]);
            var client = new Client();
            var watch = Stopwatch.StartNew();
            var request = new GetMultipleRequest { BucketName = kv[0][0], Keys = lines.ToList() };
            var result = client.GetDocuemnts(request);
            watch.Stop();

            result = result ?? new Dictionary<string, string>();

            foreach (var item in result)
                System.Console.WriteLine(item.Key + ":   " + item.Value);

            System.Console.WriteLine("{0} results returned.",result.Count);
            System.Console.WriteLine("{0} documents found, {1} documents not found", result.Count(data => !string.IsNullOrEmpty(data.Value)), result.Count(data => string.IsNullOrEmpty(data.Value)));
            System.Console.WriteLine("Operation took {0} milliseconds", watch.ElapsedMilliseconds);
        }


        static void DisplayMenu()
        {
            System.Console.WriteLine("Options");
            System.Console.WriteLine("--------------------------");
            System.Console.WriteLine("Add: add bucket|key|value");
            System.Console.WriteLine("Bulk Add: bulk add bucket|key start|from|to");
            System.Console.WriteLine("Bulk Get: bulk get bucket|key start|from|to");
            System.Console.WriteLine("Get: get bucket|key");
            System.Console.WriteLine("Del: del bucket|key");
            System.Console.WriteLine("CBucket: bucketName|maxRecordSize|maxRecordsPerBin");           
            System.Console.WriteLine("--------------------------");
            System.Console.WriteLine("file add: file");
            System.Console.WriteLine("file get: file");
            System.Console.WriteLine("-Files must be in pipe delimited bucket|key|value format.");
            System.Console.WriteLine("--------------------------");
            System.Console.WriteLine("Menu: to display menu.");
            System.Console.WriteLine("Exit: to exit application.");
        }
    }
}
