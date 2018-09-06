// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.CSharp.DataFlowAnalysis
{
    /// <summary>
    /// Interface for a traversable node.
    /// </summary>
    public interface ITraversable<T> where T : INode
    {
        /// <summary>
        /// Set of the immediate successors.
        /// </summary>
        ISet<T> ISuccessors { get; }

        /// <summary>
        /// Set of the immediate predecessors.
        /// </summary>
        ISet<T> IPredecessors { get; }

        /// <summary>
        /// Returns true if the node is a successor
        /// of the specified node.
        /// </summary>
        /// <param name="node">INode</param>
        /// <returns>Boolean</returns>
        bool IsSuccessorOf(T node);

        /// <summary>
        /// Returns true if the node is a predecessor
        /// of the specified node.
        /// </summary>
        /// <param name="node">INode</param>
        /// <returns>Boolean</returns>
        bool IsPredecessorOf(T node);
    }
}
