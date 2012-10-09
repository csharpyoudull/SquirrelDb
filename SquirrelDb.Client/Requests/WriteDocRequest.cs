// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-08-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="WriteDocRequest.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace SquirrelDb.Client.Requests
{
    /// <summary>
    /// Class WriteDocRequest
    /// </summary>
    public class WriteDocRequest
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        /// <value>The name of the bucket.</value>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WriteDocRequest" /> is update.
        /// </summary>
        /// <value><c>true</c> if update; otherwise, <c>false</c>.</value>
        public bool Update { get; set; }
    }
}
