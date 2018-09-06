// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Timers
{
    /// <summary>
    /// Unique identifier for a timer.
    /// </summary>
    public class TimerId
    {
        /// <summary>
        /// The id of the timer machine.
        /// </summary>
        internal readonly MachineId MachineId;

        /// <summary>
        /// The payload of the timer.
        /// </summary>
        public readonly object Payload;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerId"/> class.
        /// </summary>
        /// <param name="mid">The id of the timer machine.</param>
        /// <param name="payload">The payload of the timer.</param>
        internal TimerId(MachineId mid, object payload)
        {
            this.MachineId = mid;
            this.Payload = payload;
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal
        /// to the current System.Object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is TimerId tid))
            {
                return false;
            }

            return this.MachineId == tid.MachineId;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return this.MachineId.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current timer id.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Timer[{0},{1}]", this.MachineId, this.Payload != null ? this.Payload.ToString() : "null");
        }
    }
}
