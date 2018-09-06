// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Timers
{
    /// <summary>
    /// Event requesting stoppage of timer.
    /// </summary>
    internal class HaltTimerEvent : Event
    {
        /// <summary>
        /// The id of the machine invoking the request to stop the timer.
        /// </summary>
        public MachineId Client;

        /// <summary>
        /// True if the user wants to flush the client inbox of relevant timeout messages.
        /// </summary>
        public bool Flush;

        /// <summary>
        /// Initializes a new instance of the <see cref="HaltTimerEvent"/> class.
        /// </summary>
        /// <param name="client">The id of the machine invoking the request to stop the timer.</param>
        /// <param name="flush">True if the user wants to flush the client inbox of relevant timeout messages.</param>
        public HaltTimerEvent(MachineId client, bool flush)
        {
            this.Client = client;
            this.Flush = flush;
        }
    }
}
