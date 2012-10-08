﻿// ***********************************************************************
// Assembly         : SquirrelDb
// Author           : Steve
// Created          : 10-08-2012
//
// Last Modified By : Steve
// Last Modified On : 10-08-2012
// ***********************************************************************
// <copyright file="KeyTree.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SquirrelDb
{
    /// <summary>
    /// Class KeyTree
    /// </summary>
    public class KeyTree
    {
        #region private properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        public KeyNode Root { get; set; }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyTree" /> class.
        /// </summary>
        public KeyTree()
        {
        }

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>KeyTree.</returns>
        public static KeyTree CreateNew(string fileName)
        {
            var kt = new KeyTree { FileName = fileName };
            kt.Save();

            return kt;
        }

        /// <summary>
        /// Loads the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>KeyTree.</returns>
        public static KeyTree Load(string fileName)
        {
            return JsonConvert.DeserializeObject<KeyTree>(File.ReadAllText(fileName));
        }

        #endregion

        #region public methods

        /// <summary>
        /// Inserts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="pointer">The pointer.</param>
        public void Insert(long value, DataPointer pointer)
        {
            if (Root == null)
            {
                Root = new KeyNode { Value = value, Pointer = pointer };
                return;
            }

            var freeNode = SeekFreeNode(Root, value);

            if (freeNode.Value.Equals(value) && !freeNode.Pointer.Equals(pointer))
            {
                freeNode.Pointer = pointer;
                return;
            }

            if (value < freeNode.Value)
            {
                freeNode.Left = new KeyNode { Parent = freeNode, Pointer = pointer, Value = value };
                return;
            }

            freeNode.Right = new KeyNode { Parent = freeNode, Pointer = pointer, Value = value };

        }

        /// <summary>
        /// Seeks the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="value">The value.</param>
        /// <returns>KeyNode.</returns>
        public KeyNode SeekNode(KeyNode node, long value)
        {
            if (Root == null)
                return null;

            if (node == null)
                node = Root;

            if (node.Value.Equals(value))
                return node;

            if (node.Left != null && value < node.Value)
                return SeekNode(node.Left, value);

            if (node.Right != null && value > node.Value)
                return SeekNode(node.Right, value);

            return null;

        }

        /// <summary>
        /// Deletes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Delete(long value)
        {
            var node = SeekNode(null, value);

            if (node == null)
                return;

            if (node.Left == null && node.Right == null)
            {
                if (node.Parent.Left.Equals(node))
                {
                    node.Parent.Left = null;
                    return;
                }

                if (node.Parent.Right.Equals(node))
                {
                    node.Parent.Right = null;
                    return;
                }
            }

            if (node.Right != null)
            {
                if (node.Parent.Right.Equals(node))
                {
                    node.Parent.Right = node.Right;
                    if (node.Left != null)
                    {
                        var newParent = SeekFreeNode(node.Right, node.Left.Value);
                        if (node.Left.Value < newParent.Value)
                        {
                            newParent.Left = node.Left;
                            node.Left.Parent = newParent;
                        }
                        else
                        {
                            newParent.Right = node.Left;
                            node.Left.Parent = newParent;
                        }
                    }
                }
                else
                {
                    node.Parent.Right = node.Right;
                }

                return;
            }

            //left
            if (node.Left != null)
            {
                node.Parent.Right = node.Left;
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            var saveTask = new Task(() =>
            {
                lock (_saveLock)
                {
                    File.WriteAllText(FileName, JsonConvert.SerializeObject(this));
                }
            });
            saveTask.Start();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Seeks the free node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="value">The value.</param>
        /// <returns>KeyNode.</returns>
        private KeyNode SeekFreeNode(KeyNode node, long value)
        {
            if (node.Value.Equals(value))
                return node;

            if (node.Left == null && value < node.Value)
                return node;

            if (node.Right == null && value > node.Value)
                return node;

            if (node.Left != null && value < node.Value)
                return SeekFreeNode(node.Left, value);

            if (node.Right != null && value > node.Value)
                return SeekFreeNode(node.Right, value);

            return null;
        }

        /// <summary>
        /// The _save lock
        /// </summary>
        private readonly object _saveLock = new object();

        #endregion
    }
}