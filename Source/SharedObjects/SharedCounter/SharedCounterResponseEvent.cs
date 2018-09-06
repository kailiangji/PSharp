// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.SharedObjects
{
    /// <summary>
    /// Event containing the value of a shared counter.
    /// </summary>
    internal class SharedCounterResponseEvent : Event
    {
        /// <summary>
        /// Value.
        /// </summary>
        public int Value;

        /// <summary>
        /// Creates a new response event.
        /// </summary>
        /// <param name="value">Value</param>
        public SharedCounterResponseEvent(int value)
        {
            Value = value;
        }
    }
}
