// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-08-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="KeyNode.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDb
{
    /// <summary>
    /// Class KeyNode
    /// </summary>
    [Serializable]
    public class KeyNode
    {
        /// <summary>
        /// Gets or sets the pointer.
        /// </summary>
        /// <value>The pointer.</value>
        public DataPointer Pointer { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public long Value { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public KeyNode Parent { get; set; }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public KeyNode Left { get; set; }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public KeyNode Right { get; set; }
    }
}
