// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.PSharp
{
    /// <summary>
    /// The wild card event.
    /// </summary>
    [DataContract]
    public sealed class WildCardEvent : Event
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WildCardEvent()
            : base()
        {

        }
    }
}
