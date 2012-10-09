// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-08-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="GetMultipleRequest.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
namespace SquirrelDb.Client.Requests
{
    /// <summary>
    /// Class GetMultipleRequest
    /// </summary>
    public class GetMultipleRequest
    {
        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        /// <value>The name of the bucket.</value>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public List<string> Keys { get; set; } 
    }
}
