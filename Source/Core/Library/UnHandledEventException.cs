// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp
{
    /// <summary>
    /// Signals that a machine received an unhandled event
    /// </summary>
    public sealed class UnhandledEventException : RuntimeException
    {
        /// <summary>
        /// The machine that threw the exception
        /// </summary>
        public MachineId Mid;

        /// <summary>
        /// Name of the current state of the machine
        /// </summary>
        public string CurrentStateName;

        /// <summary>
        ///  The event
        /// </summary>
        public Event UnhandledEvent;

        /// <summary>
        /// Initializes a new instance of the exception.
        /// </summary>
        /// <param name="mid">MachineId</param>
        /// <param name="currentStateName">Current state name</param>
        /// <param name="unhandledEvent">The event that was unhandled</param>
        /// <param name="message">Message</param>
        internal UnhandledEventException(MachineId mid, string currentStateName, Event unhandledEvent, string message)
            : base(message)
        {
            this.Mid = mid;
            this.CurrentStateName = currentStateName;
            this.UnhandledEvent = unhandledEvent;
        }
    }
}

