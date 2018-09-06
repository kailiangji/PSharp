// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Timers
{
    /// <summary>
    /// Event used to flush the queue of a machine of eTimeout events.
    /// A single Markup event is dispatched to the queue. Then all
    /// eTimeout events are removed until we see the Markup event.
    /// </summary>
    internal class MarkupEvent : Event
    {
    }
}
