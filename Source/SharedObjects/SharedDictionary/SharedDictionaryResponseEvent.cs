// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.SharedObjects
{
    /// <summary>
    /// Event containing the value of a shared dictionary.
    /// </summary>
    internal class SharedDictionaryResponseEvent<T> : Event
    {
        /// <summary>
        /// Value.
        /// </summary>
        public T Value;

        /// <summary>
        /// Creates a new response event.
        /// </summary>
        /// <param name="value">Value</param>
        public SharedDictionaryResponseEvent(T value)
        {
            Value = value;
        }
    }
}
