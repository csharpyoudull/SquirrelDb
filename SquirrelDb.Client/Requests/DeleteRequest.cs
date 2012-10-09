// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-09-2012
//
// Last Modified By : Steve
// Last Modified On : 10-09-2012
// ***********************************************************************
// <copyright file="DeleteRequest.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace SquirrelDb.Client.Requests
{
    /// <summary>
    /// Class DeleteRequest
    /// </summary>
    public class DeleteRequest
    {
        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        /// <value>The name of the bucket.</value>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }
    }
}
