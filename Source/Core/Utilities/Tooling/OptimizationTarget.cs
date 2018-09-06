// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Utilities
{
    /// <summary>
    /// P# compilation optimization target.
    /// </summary>
    public enum OptimizationTarget
    {
        /// <summary>
        /// Enables debug optimization target.
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Enables release optimization target.
        /// </summary>
        Release = 1
    }
}
