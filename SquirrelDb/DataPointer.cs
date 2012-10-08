// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-08-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="DataPointer.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace SquirrelDb
{
    /// <summary>
    /// Struct DataPointer
    /// </summary>
    public struct DataPointer
    {
        /// <summary>
        /// Gets or sets the pointer.
        /// </summary>
        /// <value>The pointer.</value>
        public long Pointer { get; set; }

        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        /// <value>The file id.</value>
        public long FileId { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; set; }

        /// <summary>
        /// Updates the size.
        /// </summary>
        /// <param name="size">The size.</param>
        public void UpdateSize(int size)
        {
            Size = size;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return (new [] {Pointer, FileId}).GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var pointer = (DataPointer) obj;
            return pointer.FileId.Equals(FileId) && pointer.Pointer.Equals(Pointer);
        }
    }
}
