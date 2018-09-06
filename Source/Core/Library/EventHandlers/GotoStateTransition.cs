// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.PSharp
{
    /// <summary>
    /// Defines a goto state transition.
    /// </summary>
    internal sealed class GotoStateTransition
    {
        /// <summary>
        /// Target state.
        /// </summary>
        public Type TargetState;

        /// <summary>
        /// An optional lambda function, which can execute after
        /// the default OnExit function of the exiting state.
        /// </summary>
        public string Lambda;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GotoStateTransition(Type targetState, string lambda)
        {
            this.TargetState = targetState;
            this.Lambda = lambda;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GotoStateTransition(Type targetState)
        {
            this.TargetState = targetState;
            this.Lambda = null;
        }
    }
}
