using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SquirrelDb.Requests;
using System.Configuration;

namespace SquirrelDb.Test
{
    [TestClass]
    public class ServerNodeTests
    {
        [TestMethod]
        public void CreateBucket()
        {
            ServerNode.ActivateDatabase();
            var hostUrl = ConfigurationManager.AppSettings["ApiHostUrl"];
            
            var createBucket = new CreateBucketRequest
                                   {BucketName = "TestBucket", MaxRecordSize = 1024, MaxRecordsPerBin = 250000};

            var requstJson = JsonConvert.SerializeObject(createBucket);
            var request = HttpWebRequest.Create(hostUrl + "/buckets/") as HttpWebRequest;
            request.Method = "PUT";
            request.ContentLength = requstJson.Length;
            request.GetRequestStream().Write(Encoding.ASCII.GetBytes(requstJson),0,requstJson.Length);
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var message = reader.ReadToEnd();


        }
    }
}
