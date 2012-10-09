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

        /// <summary>
        /// The _map lock
        /// </summary>
        private readonly object _mapLock  = new object();

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

        #region public methods

        /// <summary>
        /// Stores the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="document">The document.</param>
        /// <exception cref="System.Exception">Document with this key already exists.</exception>
        public void Add(string key, string document)
        {
            //get a map with a free space
            var keyHash = key.GetHashCode();
            if (KeyTree.SeekNode(null,keyHash) != null)
                throw new Exception("Document with this key already exists.");

            var freeMap = MappedFiles.FirstOrDefault(mf => mf.Value.FreeBlocks.Any());
            while (freeMap.Value == null)
            {
                CreateNewMapFile();
                freeMap = MappedFiles.FirstOrDefault(mf => mf.Value.FreeBlocks.Any());
            }

            var index = freeMap.Value.Write(document);
            KeyTree.Insert(keyHash,new DataPointer{FileId = freeMap.Key,Pointer = index,Size = document.Length});
            KeyTree.Save();
        }

        /// <summary>
        /// Updates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="document">The document.</param>
        /// <exception cref="System.Exception">Document not found.</exception>
        public void Update(string key, string document)
        {
            var keyHash = key.GetHashCode();
            var keyData = KeyTree.SeekNode(null, keyHash);

            if (keyData == null)
                throw new Exception("Document not found.");

            keyData.Pointer.UpdateSize(document.Length);
            MappedFiles[keyData.Pointer.FileId].Write(keyData.Pointer.Pointer, document);
            KeyTree.Save();
            
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Exception">Document not found.</exception>
        public string Get(string key)
        {
            var keyHash = key.GetHashCode();
            var keyData = KeyTree.SeekNode(null, keyHash);

            if (keyData == null)
                return string.Empty;

            return MappedFiles[keyData.Pointer.FileId].Read(keyData.Pointer.Pointer,keyData.Pointer.Size);
        }

        /// <summary>
        /// Gets the specified keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>Dictionary{System.StringSystem.String}.</returns>
        public Dictionary<string,string> Get(List<string> keys)
        {
            return keys.AsParallel().ToDictionary(k => k, Get);
        }

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="System.Exception">Document not found.</exception>
        public void Delete(string key)
        {
            var keyHash = key.GetHashCode();
            var keyData = KeyTree.SeekNode(null, keyHash);

            if (keyData == null)
                throw new Exception("Document not found.");

            MappedFiles[keyData.Pointer.FileId].Delete(keyData.Pointer.Pointer);
            KeyTree.Delete(keyHash);
            KeyTree.Save();
            
        }

        #endregion

        #region private methods

        /// <summary>
        /// Creates the new map file.
        /// </summary>
        private void CreateNewMapFile()
        {
            lock (_mapLock)
            {
                var count = (MappedFiles.Count() + 1);
                var hash = (Name + "-" + count).GetHashCode();
                MappedFiles.Add(hash,
                                MappedFile.CreateNewMap(hash + ".bin", Location, MaxDocumentSize,
                                                        MaxDocumentCountPerFile));
            }
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
