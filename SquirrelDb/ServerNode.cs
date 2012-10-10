// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-08-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="ServerNode.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using SquirrelDb.Requests;
using System.Configuration;

namespace SquirrelDb
{
    /// <summary>
    /// Class ServerNode
    /// </summary>
    public class ServerNode:NancyModule
    {
        /// <summary>
        /// Gets or sets the buckets.
        /// </summary>
        /// <value>The buckets.</value>
        public static Dictionary<string, DataBucket> Buckets { get; set; }

        /// <summary>
        /// Gets or sets the db location.
        /// </summary>
        /// <value>The db location.</value>
        public static string DbLocation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerNode" /> class.
        /// </summary>
        public ServerNode()
        {
            Get["/documents/{bucket}/{id}"] = parameters => GetByKey(parameters["bucket"],parameters["id"]);
            Get["/buckets/"] = parameters => JsonConvert.SerializeObject(Buckets.Keys);
            Post["/documents/"] = parameters =>
                                     {
                                         var reader = new StreamReader(Request.Body);
                                         var json = reader.ReadToEnd();
                                         
                                         return GetByKeys(JsonConvert.DeserializeObject<GetMultipleRequest>(json));
                                     };
            Put["/documents/"] = parameters =>
                                    {
                                        var reader = new StreamReader(Request.Body);
                                        var json = reader.ReadToEnd();

                                        return Store(JsonConvert.DeserializeObject<WriteDocRequest>(json));
                                    };

            Put["/buckets/"] = parameters =>
                                    {
                                        var reader = new StreamReader(Request.Body);
                                        var json = reader.ReadToEnd();

                                        return CreateBucket(JsonConvert.DeserializeObject<CreateBucketRequest>(json));
                                    };
            Delete["/documents/"] = parameters =>
                                        {
                                            var reader = new StreamReader(Request.Body);
                                            var json = reader.ReadToEnd();

                                            return DeleteRecord(JsonConvert.DeserializeObject<List<DeleteRequest>>(json));
                                        };


        }

        private string DeleteRecord(List<DeleteRequest> deletes)
        {
            try
            {
                foreach (var delete in deletes)
                {
                    if (Buckets.ContainsKey(delete.BucketName))
                        Buckets[delete.BucketName].Delete(delete.Key);
                }

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Stores the specified request.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>System.String.</returns>
        private HttpStatusCode Store(WriteDocRequest request)
        {
            try
            {
                
                if (string.IsNullOrEmpty(request.BucketName))
                {
                    return HttpStatusCode.BadRequest;
                }

                if (string.IsNullOrEmpty(request.Key))
                {
                    return HttpStatusCode.BadRequest;
                }

                if (!request.Update)
                {
                    Buckets[request.BucketName.ToLower()].Add(request.Key,
                                                                request.Value);

                    return HttpStatusCode.OK;
                }

                if (request.Update)
                {
                    Buckets[request.BucketName.ToLower()].Update(request.Key,
                                                                    request.Value);

                    return HttpStatusCode.OK;
                }

                return HttpStatusCode.BadRequest;
            }
            catch(Exception ex)
            {
                return HttpStatusCode.InternalServerError;

            }
        }

        /// <summary>
        /// Gets the by key.
        /// </summary>
        /// <param name="bucket">The bucket.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        private string GetByKey(string bucket, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(bucket))
                    return string.Empty;

                if (string.IsNullOrEmpty(key))
                    return string.Empty;

                return Buckets[bucket.ToLower()].Get(key);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets the by keys.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.String.</returns>
        private string GetByKeys(GetMultipleRequest request)
        {
            try
            {

                if (string.IsNullOrEmpty(request.BucketName))
                    return string.Empty;

                if (request.Keys == null || !request.Keys.Any())
                    return string.Empty;

                var results = Buckets[request.BucketName.ToLower()].Get(request.Keys);
                return JsonConvert.SerializeObject(results);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Creates the bucket.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.String.</returns>
        public string CreateBucket(CreateBucketRequest request)
        {
            try
            {
                request.BucketName = request.BucketName.ToLower();
                if (string.IsNullOrEmpty(request.BucketName))
                    return string.Empty;

                if (Buckets.ContainsKey(request.BucketName))
                    return string.Empty;

                Buckets.Add(request.BucketName,DataBucket.CreateNewBucket(request.BucketName,DbLocation,request.MaxRecordSize,request.MaxRecordsPerBin));
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets or sets the service host.
        /// </summary>
        /// <value>The service host.</value>
        private static NancyHost ServiceHost { get; set; }
        
        /// <summary>
        /// Activates the database.
        /// </summary>
        public static void ActivateDatabase()
        {
            Buckets = new Dictionary<string, DataBucket>();
            DbLocation = ConfigurationManager.AppSettings["DatabaseLocation"];
            var buckets = Directory.GetFiles(DbLocation, "*.bucket").Select(fi => new FileInfo(fi));
            foreach (var bucket in buckets)
            {
                var bk = DataBucket.LoadDataBucket(bucket.Name.Replace(".bucket", string.Empty),DbLocation);
                Buckets.Add(bk.Name, bk);
            }

            var hostUrl = ConfigurationManager.AppSettings["ApiHostUrl"];
            ServiceHost = new NancyHost(new Uri(hostUrl));
            ServiceHost.Start();
        }
    }
}
