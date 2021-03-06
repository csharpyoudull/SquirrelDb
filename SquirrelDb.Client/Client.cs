﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SquirrelDb.Client.Requests;
using System.Web;

namespace SquirrelDb.Client
{
    public class Client
    {
        public HttpStatusCode CreateBucket(string name, int maxRecordSize, int maxRecordsPerBin)
        {
            var hostUrl = ConfigurationManager.AppSettings["ApiHostUrl"];

            var createBucket = new CreateBucketRequest
                             {BucketName = name, MaxRecordSize = maxRecordSize, MaxRecordsPerBin = maxRecordsPerBin};

            var requstJson = JsonConvert.SerializeObject(createBucket);
            var request = HttpWebRequest.Create(hostUrl + "/buckets/") as HttpWebRequest;
            request.Timeout = 400000;
            request.Method = "PUT";
            request.ContentLength = requstJson.Length;
            request.GetRequestStream().Write(Encoding.ASCII.GetBytes(requstJson), 0, requstJson.Length);
            var response = request.GetResponse() as HttpWebResponse;
            return response.StatusCode;
        }

        public HttpStatusCode StoreDocument(WriteDocRequest documents)
        {
            var hostUrl = ConfigurationManager.AppSettings["ApiHostUrl"];

            var requstJson = JsonConvert.SerializeObject(documents);
            var request = HttpWebRequest.Create(hostUrl + "/documents/") as HttpWebRequest;
            request.Timeout = 400000;
            request.Method = "PUT";
            request.ContentLength = requstJson.Length;
            request.GetRequestStream().Write(Encoding.ASCII.GetBytes(requstJson), 0, requstJson.Length);
            var response = request.GetResponse() as HttpWebResponse;

            return response.StatusCode;
        }

        public HttpStatusCode DeleteDocument(DeleteRequest document)
        {
            var hostUrl = ConfigurationManager.AppSettings["ApiHostUrl"];

            var requstJson = JsonConvert.SerializeObject(document);
            var request = HttpWebRequest.Create(hostUrl + "/documents/") as HttpWebRequest;
            request.Timeout = 400000;
            request.Method = "DELETE";
            request.ContentLength = requstJson.Length;
            request.GetRequestStream().Write(Encoding.ASCII.GetBytes(requstJson), 0, requstJson.Length);
            var response = request.GetResponse() as HttpWebResponse;

            return response.StatusCode;
        }

        public string GetDocument(string bucket, string key)
        {
            var hostUrl = ConfigurationManager.AppSettings["ApiHostUrl"];
            
            var request = HttpWebRequest.Create(hostUrl + string.Format("/documents/{0}/{1}",bucket,HttpUtility.UrlEncode(key))) as HttpWebRequest;
            request.Timeout = 400000;
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var message = reader.ReadToEnd();

            return message;
        }

        public Dictionary<string,string> GetDocuemnts(GetMultipleRequest getRequest)
        {
            var hostUrl = ConfigurationManager.AppSettings["ApiHostUrl"];

            var requstJson = JsonConvert.SerializeObject(getRequest);
            var request = HttpWebRequest.Create(hostUrl + "/documents/") as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = requstJson.Length;
            request.GetRequestStream().Write(Encoding.ASCII.GetBytes(requstJson), 0, requstJson.Length);
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var message = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
        }
    }
}
