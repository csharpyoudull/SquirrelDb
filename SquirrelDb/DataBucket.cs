// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-01-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="DataBucket.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SquirrelDb
{
    /// <summary>
    /// Class DataBucket
    /// </summary>
    public class DataBucket
    {
        #region public properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the mapped files.
        /// </summary>
        /// <value>The mapped files.</value>
        [JsonIgnore]
        public Dictionary<long,MappedFile> MappedFiles { get; set; }

        /// <summary>
        /// Gets or sets the size of the max document.
        /// </summary>
        /// <value>The size of the max document.</value>
        public int MaxDocumentSize { get; set; }

        /// <summary>
        /// Gets or sets the max document count per file.
        /// </summary>
        /// <value>The max document count per file.</value>
        public int MaxDocumentCountPerFile { get; set; }

        #endregion

        #region private properties

        /// <summary>
        /// Lock for saving data to avoid multi-thread file access errors.
        /// </summary>
        private readonly object _saveLock = new object();

        /// <summary>
        /// Gets or sets the key tree.
        /// </summary>
        /// <value>The key tree.</value>
        [JsonIgnore]
        private KeyTree KeyTree { get; set; }

        #endregion

        #region constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="DataBucket" /> class from being created.
        /// </summary>
        private DataBucket()
        {
            MappedFiles = new Dictionary<long, MappedFile>();
        }

        /// <summary>
        /// Creates the new bucket.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="location">The location.</param>
        /// <param name="maxDocumentSize">Size of the max document.</param>
        /// <param name="maxDocumentCountPerFile">The max document count per file.</param>
        /// <returns>DataBucket.</returns>
        public static DataBucket CreateNewBucket(string name, string location, int maxDocumentSize, int maxDocumentCountPerFile)
        {
            var bucket = new DataBucket
                             {
                                 Name = name,
                                 Location = Path.Combine(location,name),
                                 MaxDocumentSize = maxDocumentSize,
                                 MaxDocumentCountPerFile = maxDocumentCountPerFile
                             };

            if (!Directory.Exists(bucket.Location))
                Directory.CreateDirectory(bucket.Location);

            bucket.CreateNewMapFile();
            bucket.KeyTree = KeyTree.CreateNew(Path.Combine(bucket.Location, bucket.Name + "-KeyTree.key"));
            File.WriteAllText(Path.Combine(location,name + ".bucket"),bucket.ToString());

            return bucket;
        }

        /// <summary>
        /// Loads the data bucket.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="location">The location.</param>
        /// <returns>DataBucket.</returns>
        public static DataBucket LoadDataBucket(string name, string location)
        {
            var bucket =
                JsonConvert.DeserializeObject<DataBucket>(File.ReadAllText(Path.Combine(location, name + ".bucket")));

            var files = Directory.GetFiles(bucket.Location);
            var fileInfo = files.Select(fi => new FileInfo(fi)).Where(fi => fi.Extension.Equals(".bin",StringComparison.OrdinalIgnoreCase ));
            bucket.MappedFiles =
                fileInfo.Select(fi => new KeyValuePair<long, MappedFile>(long.Parse(fi.Name.Replace(".bin",string.Empty)), MappedFile.LoadMap(fi.Name, bucket.Location)))
                    .ToDictionary(k => k.Key, v => v.Value);
            bucket.KeyTree = KeyTree.Load(Path.Combine(bucket.Location, bucket.Name + "-KeyTree.key"));
            return bucket;
        }

        #endregion

        /// <summary>
        /// Stores the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="document">The document.</param>
        public void Store(string key, string document)
        {

        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string Get(string key)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the specified keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>Dictionary{System.StringSystem.String}.</returns>
        public Dictionary<string,string> Get(List<string> keys)
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Delete(string key)
        {

        }

        #region private methods

        /// <summary>
        /// Creates the new map file.
        /// </summary>
        private void CreateNewMapFile()
        {
            var count = (MappedFiles.Count() + 1);
            var hash = (Name + "-" + count).GetHashCode();
            MappedFiles.Add(hash,MappedFile.CreateNewMap(hash + ".bin",Location,MaxDocumentSize,MaxDocumentCountPerFile));
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        private void Save()
        {
            var saveTask = new Task(() =>
            {
                lock (_saveLock)
                {
                    var configPath = Path.Combine(Location, Name + ".bucket");
                    File.WriteAllText(configPath, ToString());
                }
            });
            saveTask.Start();
        }

        #endregion

        #region overrides

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
