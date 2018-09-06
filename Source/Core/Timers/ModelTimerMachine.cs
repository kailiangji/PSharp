// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.Timers
{
    /// <summary>
    /// A timer model, used for testing purposes.
    /// </summary>
    public class ModelTimerMachine : Machine
    {
        /// <summary>
        /// Adjust the probability of firing a timeout event.
        /// </summary>
        public static int NumStepsToSkip = 1;

        /// <summary>
        /// The id of the machine to which eTimeout events are dispatched.
        /// </summary>
        private MachineId client;

        /// <summary>
        /// The id of the timer.
        /// </summary>
        private TimerId TimerId;

        /// <summary>
        /// True if periodic eTimeout events are desired.
        /// </summary>
        private bool IsPeriodic;

        /// <summary>
        /// The only state of the timer.
        /// </summary>
        [Start]
        [OnEntry(nameof(InitializeTimer))]
        [OnEventDoAction(typeof(HaltTimerEvent), nameof(DisposeTimer))]
        [OnEventDoAction(typeof(RepeatTimeoutEvent), nameof(SendTimeout))]
        private class Init : MachineState
        {
        }

        /// <summary>
        /// Initializes the timer.
        /// </summary>
        private void InitializeTimer()
        {
            InitTimerEvent e = this.ReceivedEvent as InitTimerEvent;
            this.client = e.Client;
            this.IsPeriodic = e.IsPeriodic;
            this.TimerId = e.TimerId;
            this.Send(this.Id, new RepeatTimeoutEvent());
        }

        /// <summary>
        /// Sends a timeout event.
        /// </summary>
        private void SendTimeout()
        {
            this.Assert(NumStepsToSkip >= 0);

            // If not periodic, send a single timeout event.
            if (!this.IsPeriodic)
            {
                // Probability of firing timeout is atmost 1/N.
                if ((this.RandomInteger(NumStepsToSkip) == 0) && this.FairRandom())
                {
                    this.Send(this.client, new TimerElapsedEvent(this.TimerId));
                }
                else
                {
                    this.Send(this.Id, new RepeatTimeoutEvent());
                }
            }
            else
            {
                // Probability of firing timeout is atmost 1/N.
                if ((this.RandomInteger(NumStepsToSkip) == 0) && this.FairRandom())
                {
                   this.Send(this.client, new TimerElapsedEvent(this.TimerId));
                }

                this.Send(this.Id, new RepeatTimeoutEvent());
            }
        }

        /// <summary>
        /// Disposes the timer.
        /// </summary>
        private void DisposeTimer()
        {
            HaltTimerEvent e = this.ReceivedEvent as HaltTimerEvent;

            // The client attempting to stop this timer must be the one who created it.
            this.Assert(e.Client == this.client);

            // If the client wants to flush the inbox, send a markup event.
            // This marks the endpoint of all timeout events sent by this machine.
            if (e.Flush)
            {
                this.Send(this.client, new MarkupEvent());
            }

            // Stop this machine.
            this.Raise(new Halt());
        }
    }
}
