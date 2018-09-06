// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Timers
{
    /// <summary>
    /// Timeout event sent by the timer.
    /// </summary>
    public class TimerElapsedEvent : Event
    {
        /// <summary>
        /// The id of the timer.
        /// </summary>
        public readonly TimerId Tid;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerElapsedEvent"/> class.
        /// </summary>
        /// <param name="tid">The id of the timer.</param>
        public TimerElapsedEvent(TimerId tid)
        {
            this.Tid = tid;
        }
    }
}
