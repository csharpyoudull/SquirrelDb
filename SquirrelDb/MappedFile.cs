// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve Ruben @CSharpYouDull - twitter
// Created          : 09-28-2012
//
// Last Modified By : Steve Ruben
// Last Modified On : 09-28-2012
// ***********************************************************************
// <copyright file="MappedFile.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SquirrelDb
{
    /// <summary>
    /// Class MappedFile
    /// </summary>
    public class MappedFile
    {
        #region public properties

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location { get; set; }

        /// <summary>
        /// Gets the size of the max block.
        /// </summary>
        /// <value>The size of the max block.</value>
        public int MaxBlockSize { get; set; }

        /// <summary>
        /// Gets the map file.
        /// </summary>
        /// <value>The map file.</value>
        [JsonIgnore]
        public MemoryMappedFile MapFile { get; private set; }

        /// <summary>
        /// Gets the free blocks.
        /// </summary>
        /// <value>The free blocks.</value>
        public List<long> FreeBlocks { get; set; }

        /// <summary>
        /// Gets or sets the blocks free.
        /// </summary>
        /// <value>The blocks free.</value>
        public long BlocksFree { get; set; }

        #endregion

        #region private properties

        /// <summary>
        /// Lock for write or delete operations
        /// </summary>
        private readonly object _writeLock = new object();

        /// <summary>
        /// Lock for saving data to avoid multi-thread file access errors.
        /// </summary>
        private readonly object _saveLock = new object();

        /// <summary>
        /// The _map lock
        /// </summary>
        private static readonly object MapLock = new object();

        #endregion

        #region constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="MappedFile" /> class from being created.
        /// </summary>
        private MappedFile()
        {
            FreeBlocks = new List<long>();
        }

        /// <summary>
        /// Creates the new map.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="location">The location.</param>
        /// <param name="maxBlockSize">Size of the max block.</param>
        /// <param name="maxBlocksPerFile">The max blocks per file.</param>
        /// <returns>MappedFile.</returns>
        public static MappedFile CreateNewMap(string fileName, string location, int maxBlockSize, int maxBlocksPerFile)
        {
            lock (MapLock)
            {
                var map = new MappedFile
                              {
                                  FileName = fileName,
                                  Location = location,
                                  MaxBlockSize = maxBlockSize,
                                  MapFile =
                                      MemoryMappedFile.CreateFromFile(Path.Combine(location, fileName),
                                                                      FileMode.CreateNew,
                                                                      fileName, maxBlockSize*maxBlocksPerFile)
                              };

                for (var i = 0; i < maxBlocksPerFile; i++)
                {
                    map.FreeBlocks.Add(i);
                }

                map.BlocksFree = maxBlocksPerFile;
                var configPath = Path.Combine(location, fileName + ".config");
                File.WriteAllText(configPath, map.ToString());

                return map;
            }
        }

        /// <summary>
        /// Loads the map.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="location">The location.</param>
        /// <returns>MappedFile.</returns>
        public static MappedFile LoadMap(string fileName, string location)
        {
            var configPath = Path.Combine(location, fileName + ".config");
            var map = JsonConvert.DeserializeObject<MappedFile>(File.ReadAllText(configPath));
            map.MapFile = MemoryMappedFile.CreateFromFile(Path.Combine(location, fileName), FileMode.Open);

            return map;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int64.</returns>
        public long Write(string value)
        {
            lock(_writeLock)
            {
                if (BlocksFree <= 0)
                    return -1;

                var freeBlock = FreeBlocks.First();
                using (var accessor = MapFile.CreateViewAccessor(freeBlock * MaxBlockSize, MaxBlockSize))
                {
                    accessor.WriteArray(0, Encoding.ASCII.GetBytes(value), 0, value.Length);
                }

                FreeBlocks = FreeBlocks.Where(block => !block.Equals(freeBlock)).ToList();
                BlocksFree--;
                Save();
                return freeBlock;
            }
        }

        /// <summary>
        /// Writes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Write(long index, string value)
        {   
            using (var accessor = MapFile.CreateViewAccessor(index * MaxBlockSize, MaxBlockSize))
            {
                accessor.WriteArray(0, Encoding.ASCII.GetBytes(value), 0, value.Length);
            }
        }

        /// <summary>
        /// Reads the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>System.String.</returns>
        public string Read(long position)
        {
            var data = new byte[MaxBlockSize];
            using (var accessor = MapFile.CreateViewAccessor(position * MaxBlockSize, MaxBlockSize))
            {
                accessor.ReadArray(0, data, 0, data.Length);
            }

            return Encoding.ASCII.GetString(data).TrimEnd();
        }

        /// <summary>
        /// Reads the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public string Read(long position, long length)
        {
            var data = new byte[length];
            using (var accessor = MapFile.CreateViewAccessor(position * MaxBlockSize, length))
            {
                accessor.ReadArray(0, data, 0, data.Length);
            }
            return Encoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Deletes the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void Delete(long position)
        {
            lock (_writeLock)
            {
                if (FreeBlocks.Any(fb => fb.Equals(position))) return;
                Write(position, Encoding.ASCII.GetString(new byte[MaxBlockSize]));
                FreeBlocks.Add(position);
                BlocksFree++;
                Save();
            }

        }

        #endregion

        #region private methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        private void Save()
        {
            var saveTask = new Task(() =>
            {
                lock (_saveLock)
                {
                    var configPath = Path.Combine(Location, FileName + ".config");
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
