// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Timers
{
    /// <summary>
    /// Event that initializes a timer.
    /// </summary>
    internal class InitTimerEvent : Event
    {
        /// <summary>
        /// The id of the machine creating the timer.
        /// </summary>
        public MachineId Client;

        /// <summary>
        /// The id of the timer.
        /// </summary>
        public TimerId TimerId;

        /// <summary>
        /// True if periodic timeout events are desired.
        /// </summary>
        public bool IsPeriodic;

        /// <summary>
        /// The timeout period.
        /// </summary>
        public int Period;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitTimerEvent"/> class.
        /// </summary>
        /// <param name="client">The id of the machine creating the timer.</param>
        /// <param name="tid">The id of the timer.</param>
        /// <param name="isPeriodic">True if periodic timeout events are desired.</param>
        /// <param name="period">The timeout period.</param>
        public InitTimerEvent(MachineId client, TimerId tid, bool isPeriodic, int period)
        {
            this.Client = client;
            this.TimerId = tid;
            this.IsPeriodic = isPeriodic;
            this.Period = period;
        }
    }
}
