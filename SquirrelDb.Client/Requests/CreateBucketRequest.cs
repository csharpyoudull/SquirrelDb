// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-08-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="CreateBucketRequest.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace SquirrelDb.Client.Requests
{
    /// <summary>
    /// Class CreateBucketRequest
    /// </summary>
    public class CreateBucketRequest
    {
        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        /// <value>The name of the bucket.</value>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the max records per bin.
        /// </summary>
        /// <value>The max records per bin.</value>
        public int MaxRecordsPerBin { get; set; }

        /// <summary>
        /// Gets or sets the size of the max record.
        /// </summary>
        /// <value>The size of the max record.</value>
        public int MaxRecordSize { get; set; }
    }
}
