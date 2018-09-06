// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Utilities
{
    /// <summary>
    /// Type of reduction strategy.
    /// </summary>
    public enum ReductionStrategy
    {
        /// <summary>
        /// No reduction.
        /// </summary>
        None = 0,

        /// <summary>
        /// Reduction strategy that omits scheduling points.
        /// </summary>
        OmitSchedulingPoints,

        /// <summary>
        /// Reduction strategy that forces scheduling points.
        /// </summary>
        ForceSchedule
    }
}
